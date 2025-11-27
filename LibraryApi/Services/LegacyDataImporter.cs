using Microsoft.Data.Sqlite;

public class LegacyDataImporter
{
    private readonly NewDbContext _cleanContext;

    public LegacyDataImporter(NewDbContext cleanContext)
    {
        _cleanContext = cleanContext;
    }
    
    
    public async Task ImportEmployees()
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
            DateTime? hireDateResult = null;
            DateTime tryDateResult;
            DateTime? createdOnResult;
            DateTime tryCreatedOn;
            DateTime? modifiedOnResult;
            DateTime tryModifiedOn;

            // this is to handle the easter egg mission where an employee has literally - invalid date - as their start date
            // had to run some re migrations and updates for the database to incorporate null for the startdate DateTime field
            bool hireDateSuccess = DateTime.TryParse(hireDateStr, out tryDateResult);
            hireDateResult = hireDateSuccess ? tryDateResult : null;
            bool createdSuccess = DateTime.TryParse(createdOn, out tryCreatedOn);
            createdOnResult = createdSuccess ? tryCreatedOn : null;
            bool modifiedSuccess = DateTime.TryParse(modifiedOn, out tryModifiedOn);
            modifiedOnResult = modifiedSuccess ? tryModifiedOn : null;
    
            // parsing to integers and decimals
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

            // Check if DepartmentCode exists in Departments table
            var validDept = _cleanContext.Departments.Any(d => d.DepartmentCode == deptCode);
            var safeDeptCode = validDept ? deptCode : null;

            // Check if ManagerNum exists in Employees table
            int? safeManagerNum = null;
            if (manager && _cleanContext.Employees.Any(e => e.Id == numResult))
            {
                safeManagerNum = numResult;
            }
            // the new model
            if (!_cleanContext.Employees.Any(e => e.Id == empNumResult))
            {
                var employee = new EmployeeData
                {
                    Id = empNumResult,
                    FirstName = firstName,
                    LastName = lastName,
                    DepartmentCode = safeDeptCode,
                    Salary = salaryResult,
                    ManagerNum = safeManagerNum,
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
    }

    // we do the same again - parsing info from departments now
    public async Task ImportDepartments()
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
                    DepartmentManagerNum = safeManagerNum,
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
            DateTime? start;
            DateTime? end;
            DateTime tempStart;
            DateTime tempEnd;
            bool stdate = DateTime.TryParse(startDate, out tempStart);
            bool endate = DateTime.TryParse(endDate, out tempEnd);
            start = stdate ? tempStart : null;
            end = endate ? tempEnd : null;

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
}