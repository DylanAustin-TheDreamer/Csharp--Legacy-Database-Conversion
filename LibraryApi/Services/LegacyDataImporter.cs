using Microsoft.Data.Sqlite;

public class LegacyDataImporter
{
    private readonly NewDbContext _cleanContext;

    public LegacyDataImporter(NewDbContext cleanContext)
    {
        _cleanContext = cleanContext;
    }
    
    public async Task CheckDepartmentManager()
    {
        foreach (var dept in _cleanContext.Departments)
        {
            if (dept.DepartmentManagerNum.HasValue)
            {
                var manager = _cleanContext.Employees.FirstOrDefault(e => e.Id == dept.DepartmentManagerNum.Value);
                if (manager != null)
                {
                    // Manager exists, nothing to do (EF will link via FK)
                }
                else
                {
                    // Manager does not exist, set to null
                    dept.DepartmentManagerNum = null;
                }
            }
        }
        await _cleanContext.SaveChangesAsync();
    }
    
    public async Task ImportAll()
    {
        // Pass 1: Import employees and departments with basic info only
        await ImportEmployeesBasic();
        await ImportDepartmentsBasic();
        // Pass 2: Fix-up relationships
        await FixupEmployeeDepartmentsAndManagers();
        await FixupDepartmentManagers();
    }

    private async Task ImportEmployeesBasic()
    {
        // create the connection to legacy
        var connectionString = "Data Source=../LegacyDatabase/legacy.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        // use SQL to get the employee information from legacy
        var sql = "SELECT * FROM EMPLOYEE_MASTER_TBL";
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // 2. Read messy data
        using var reader = command.ExecuteReader();

        // 3. For each row:
        //    - Parse/transform
        //    - Create EmployeeData object
        //    - Add to _cleanContext

        // this is for the second loop to reference
        var employeeList = new List<(int id, int? managerNum)>();

        while(reader.Read())
        {
            // C# is setup to handle performance in this while loop scenario. And considering the legacy db is small, this is okay.
            var empNum = reader["EMP_NUM"].ToString();
            var fullName = reader["FULL_NM"].ToString();
            var deptCode = reader["DEPT_CD"].ToString();
            var hireDateStr = reader["HIRE_DT"].ToString();
            var salaryStr = reader["SAL_AMT"].ToString();
            var statusFlag = reader["STATUS_FLG"].ToString();
            var managerNum = reader["MGR_EMP_NUM"].ToString();
            var emailAddress = reader["EMAIL_ADDR"].ToString();
            var phoneNumber = reader["PHONE_NBR"].ToString();
            var createdBy = reader["CREATED_BY"].ToString();
            var createdOn = reader["CREATED_DT"].ToString();
            var modifiedBy = reader["MODIFIED_BY"].ToString();
            var modifiedOn = reader["MODIFIED_DT"].ToString();

            // changed these to nullable to take away yellow warning
            // Here I get full name and split by comma - then create first name and last name
            var parts = fullName?.Split(",");
            // here I am handling a middle name scenario and I am just adding any other middle names to first name
            var lastName = (parts != null && parts.Length > 0) ? parts[0].Trim() : "";
            // this code skips the first element in the array and joins all other elements - giving us the first and middle name scenario
            var firstName = (parts != null && parts.Length > 1) ? string.Join(" ", parts.Skip(1)).Trim() : "";
            // parsing strings and converting to datetime
            string[] formats = { "MM/dd/yyyy", "yyyy-MM-dd", "M/d/yyyy", "dd/MM/yyyy" };
            DateTime? hireDateResult = null;
            DateTime? createdOnResult = null;
            DateTime? modifiedOnResult = null;
            DateTime tempDate;

            // I had AI implement the logic here because I got a little brain sore - have pity on me
            // Not too sure exactly what is happening but I can read over it. It uses the string array created above
            // bear in mind I have only been learning this as I go along for 3 days or something
            // Robust date parsing for hireDateStr
            if (!string.IsNullOrWhiteSpace(hireDateStr))
            {
                foreach (var fmt in formats)
                {
                    // example: I didn't know TryParseExact and what it does.
                    // it expects the input to be in year-month-day order. If the input matches, it parses it correctly and stores it as a DateTime object.
                    if (DateTime.TryParseExact(hireDateStr.Trim(), fmt, null, System.Globalization.DateTimeStyles.None, out tempDate))
                    {
                        // Once parsed, the DateTime object itself is not tied to any string format—it’s just a date value. 
                        // When you display or serialize it, you can choose the format you want (e.g., ISO, UK, US) using .ToString("yyyy-MM-dd") 
                        // or similar.
                        hireDateResult = tempDate;
                        break;

                        // So, the format strings in TryParseExact tell .NET how to interpret the incoming string, but the actual DateTime value 
                        // is always stored in a standard way internally.

                    }
                }
                if (hireDateResult == null && DateTime.TryParse(hireDateStr.Trim(), out tempDate))
                {
                    hireDateResult = tempDate;
                }
            }
            // Robust date parsing for createdOn
            if (!string.IsNullOrWhiteSpace(createdOn))
            {
                foreach (var fmt in formats)
                {
                    if (DateTime.TryParseExact(createdOn.Trim(), fmt, null, System.Globalization.DateTimeStyles.None, out tempDate))
                    {
                        createdOnResult = tempDate;
                        break;
                    }
                }
                if (createdOnResult == null && DateTime.TryParse(createdOn.Trim(), out tempDate))
                {
                    createdOnResult = tempDate;
                }
            }
            // Robust date parsing for modifiedOn
            if (!string.IsNullOrWhiteSpace(modifiedOn))
            {
                foreach (var fmt in formats)
                {
                    if (DateTime.TryParseExact(modifiedOn.Trim(), fmt, null, System.Globalization.DateTimeStyles.None, out tempDate))
                    {
                        modifiedOnResult = tempDate;
                        break;
                    }
                }
                if (modifiedOnResult == null && DateTime.TryParse(modifiedOn.Trim(), out tempDate))
                {
                    modifiedOnResult = tempDate;
                }
            }
    
            // parsing to integers and decimals
            // Parse salary string before conversion
            salaryStr = salaryStr?.Replace("$", "").Replace(",", "");
            decimal? salaryResult = null;
            decimal tempResult = 0;
            var salary = decimal.TryParse(salaryStr, out tempResult);
            salaryResult = salary ? tempResult : null;
            // for employee number
            int empNumResult;
            var employeeCheck = int.TryParse(empNum, out empNumResult);
            // for manager number
            int numResult;
            var manager = int.TryParse(managerNum, out numResult);

            // made a mistake when creating the status field in my model - going to try and salvage the situation (srry)
            // my plan is do detect the code and save either true or false.
            // my thinking at the time when I was confused was to create a bool to say if they were active or not - tried to keep things simple.
            bool status = statusFlag == "A";

            // update the employee list reference
            int? managerNumResult = manager ? numResult : (int?)null;
            employeeList.Add((empNumResult, managerNumResult));

            // the new model
            if (!_cleanContext.Employees.Any(e => e.Id == empNumResult))
            {
                var employee = new EmployeeData
                {
                    Id = empNumResult,
                    FirstName = firstName,
                    LastName = lastName,
                    // no manager code or department codes for starters - load order issue
                    Salary = salaryResult,
                    PhoneNum = phoneNumber,
                    Email = emailAddress,
                    CreatedBy = createdBy,
                    ModifiedBy = modifiedBy,
                    StatusFlag = status,
                    HireDate = hireDateResult,
                    CreatedAt = createdOnResult,
                    ModifiedAt = modifiedOnResult,
                };
                _cleanContext.Employees.Add(employee);
            }
        }
        // 4. SaveChanges
        await _cleanContext.SaveChangesAsync();

        // Second pass: update ManagerNum for each employee
        foreach (var (id, managerNum) in employeeList)
        {
            var employee = _cleanContext.Employees.FirstOrDefault(e => e.Id == id);
            if (employee != null && managerNum.HasValue)
            {
                // Only set if manager exists
                if (_cleanContext.Employees.Any(e => e.Id == managerNum.Value))
                {
                    employee.ManagerNum = managerNum.Value;
                }
            }
        }
        await _cleanContext.SaveChangesAsync();
    }

    // we do the same again - parsing info from departments now
    private async Task ImportDepartmentsBasic()
    {
        // create the connection to legacy
        var connectionString = "Data Source=../LegacyDatabase/legacy.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        // use SQL to get the department information from legacy
        var sql = "SELECT * FROM DEPT_LKP_TBL";
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // 2. Read messy data
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var departmentCode = reader["DEPT_CD"].ToString();
            var departmentName = reader["DEPT_NM"].ToString();
            var departmentManager = reader["DEPT_MGR_EMP"].ToString();
            var budgetAmount = reader["BUDGET_AMT"].ToString();
            var active = reader["ACTIVE_YN"].ToString();

            // do info parse and format
            // this for int parse for the manager number
            int? numResult;
            int tempManagerNum;
            var managerNum = int.TryParse(departmentManager, out tempManagerNum);
            numResult = managerNum ? tempManagerNum : null;

            // budget amount - set as non-nullable
            decimal? amountResult;
            decimal tempAmountResult;
            var salary = decimal.TryParse(budgetAmount, out tempAmountResult);
            amountResult = salary ? tempAmountResult : null;

            // for bool is active
            bool isActive = active == "Y" ? true : false;

            // After parsing numResult for manager number
            int? safeManagerNum = null;
            if (numResult.HasValue && _cleanContext.Employees.Any(e => e.Id == numResult.Value))
            {
                safeManagerNum = numResult.Value;
            }

            // Only add departments with unique DepartmentCode. This will prevent the UNIQUE constraint error.
            if (!_cleanContext.Departments.Any(d => d.DepartmentCode == departmentCode))
            {
                var departments = new Departments
                {
                    DepartmentCode = departmentCode,
                    DepartmentName = departmentName,
                    // no assigning manager number - load order issue
                    BudgetAmount = amountResult,
                    IsActive = isActive
                };
                _cleanContext.Departments.Add(departments);
            }
        }
        // 4. SaveChanges
        await _cleanContext.SaveChangesAsync();
    }
    
    // we do the same again - parsing info from projects now
    public async Task ImportProjects()
    {
        // create the connection to legacy
        var connectionString = "Data Source=../LegacyDatabase/legacy.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        // use SQL to get the department information from legacy
        var sql = "SELECT * FROM PROJ_ASSIGN_TBL";
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // 2. Read messy data
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var assingId = reader["ASSIGN_ID"].ToString();
            var employeeNum = reader["EMP_NUM"].ToString();
            var projectCode = reader["PROJ_CD"].ToString();
            var startDate = reader["START_DT"].ToString();
            var endDate = reader["END_DT"].ToString();
            var hrsPerWeek = reader["HOURS_PER_WK"].ToString();
            var billRate = reader["BILL_RATE"].ToString();
            var notes = reader["NOTES"].ToString();
            
            // do info parse and format
            int result;
            bool number = int.TryParse(employeeNum, out result);
            // for hours
            decimal? hrsResult;
            decimal tempResult;
            bool hrs = decimal.TryParse(hrsPerWeek, out tempResult);
            hrsResult = hrs ? tempResult : null;
            // for bill rate
            decimal? billResult;
            decimal tempBill;
            bool bill = decimal.TryParse(billRate, out tempBill);
            billResult = bill ? tempBill : null;

            // for DateTimes
            string[] formats = { "MM/dd/yyyy", "yyyy-MM-dd", "M/d/yyyy", "dd/MM/yyyy" };
            DateTime? start = null;
            DateTime? end = null;
            DateTime tempDate;
            // Robust date parsing for startDate
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                foreach (var fmt in formats)
                {
                    if (DateTime.TryParseExact(startDate.Trim(), fmt, null, System.Globalization.DateTimeStyles.None, out tempDate))
                    {
                        start = tempDate;
                        break;
                    }
                }
                if (start == null && DateTime.TryParse(startDate.Trim(), out tempDate))
                {
                    start = tempDate;
                }
            }
            // Robust date parsing for endDate
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                foreach (var fmt in formats)
                {
                    if (DateTime.TryParseExact(endDate.Trim(), fmt, null, System.Globalization.DateTimeStyles.None, out tempDate))
                    {
                        end = tempDate;
                        break;
                    }
                }
                if (end == null && DateTime.TryParse(endDate.Trim(), out tempDate))
                {
                    end = tempDate;
                }
            }

            int? safeEmployeeNum = null;
            if (_cleanContext.Employees.Any(e => e.Id == result))
            {
                safeEmployeeNum = result;
            }
            if (!_cleanContext.ProjectAssignments.Any(p => p.AssignId == assingId))
            {
                var projects = new ProjectAssignTable
                {
                    AssignId = assingId,
                    EmployeeNum = safeEmployeeNum,
                    ProjectCode = projectCode,
                    StartDate = start,
                    EndDate = end,
                    HrsPerWeek = hrsResult,
                    BillRate = billResult,
                    Notes = notes
                    
                };
                _cleanContext.ProjectAssignments.Add(projects); 
            }
        }
        await _cleanContext.SaveChangesAsync();
    }

    // Ai's contribution - things were getting bad due to the load order. I'm too new to grasp it. Things not loading on time and foreign key issues
    // The models are fine though it is just the load order
    public async Task FixupEmployeeDepartmentsAndManagers()
    {
            // Re-import legacy employee data to set DepartmentCode and ManagerNum
            var connectionString = "Data Source=../LegacyDatabase/legacy.db";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var sql = "SELECT * FROM EMPLOYEE_MASTER_TBL";
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var empNum = reader["EMP_NUM"].ToString();
                var deptCode = reader["DEPT_CD"].ToString();
                var managerNum = reader["MGR_EMP_NUM"].ToString();
                int empNumResult;
                if (int.TryParse(empNum, out empNumResult))
                {
                    var employee = _cleanContext.Employees.FirstOrDefault(e => e.Id == empNumResult);
                    if (employee != null)
                    {
                        // Set DepartmentCode if department exists
                        if (!string.IsNullOrEmpty(deptCode) && _cleanContext.Departments.Any(d => d.DepartmentCode == deptCode))
                        {
                            employee.DepartmentCode = deptCode;
                        }
                        else
                        {
                            employee.DepartmentCode = null;
                        }
                        // Set ManagerNum if manager exists
                        int managerNumResult;
                        if (int.TryParse(managerNum, out managerNumResult) && _cleanContext.Employees.Any(e => e.Id == managerNumResult))
                        {
                            employee.ManagerNum = managerNumResult;
                        }
                        else
                        {
                            employee.ManagerNum = null;
                        }
                    }
                }
            }
            await _cleanContext.SaveChangesAsync();
    }

    public async Task FixupDepartmentManagers()
    {
            // Re-import legacy department data to set DepartmentManagerNum
            var connectionString = "Data Source=../LegacyDatabase/legacy.db";
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var sql = "SELECT * FROM DEPT_LKP_TBL";
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var departmentCode = reader["DEPT_CD"].ToString();
                var departmentManager = reader["DEPT_MGR_EMP"].ToString();
                int tempManagerNum;
                int? safeManagerNum = null;
                if (int.TryParse(departmentManager, out tempManagerNum) && _cleanContext.Employees.Any(e => e.Id == tempManagerNum))
                {
                    safeManagerNum = tempManagerNum;
                }
                var dept = _cleanContext.Departments.FirstOrDefault(d => d.DepartmentCode == departmentCode);
                if (dept != null)
                {
                    dept.DepartmentManagerNum = safeManagerNum;
                }
            }
            await _cleanContext.SaveChangesAsync();
    }
}