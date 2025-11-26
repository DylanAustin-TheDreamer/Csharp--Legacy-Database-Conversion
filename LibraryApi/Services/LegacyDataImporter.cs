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
            DateTime hireDateResult;
            DateTime createdOnResult;
            DateTime modifiedOnResult;
            
            bool hireDateSuccess = DateTime.TryParse(hireDateStr, out hireDateResult);
            bool createdSuccess = DateTime.TryParse(createdOn, out createdOnResult);
            bool modifiedSuccess = DateTime.TryParse(modifiedOn, out modifiedOnResult);
    
            // parsing to integers and decimals
            decimal? salaryResult = null;
            decimal tempResult = 0;
            var salary = decimal.TryParse(salaryStr, out tempResult);
            if(salary)
            {
                // parsing success
                salaryResult = tempResult;
            }
            else
            {
                salaryResult = null;
            }
            int numResult;
            var manager = int.TryParse(managerNum, out numResult);

            // made a mistake when creating the status field in my model - going to try and salvage the situation (srry)
            // my plan is do detect the code and save either true or false.
            // my thinking at the time when I was confused was to create a bool to say if they were active or not - tried to keep things simple.
            bool status = statusFlag == "A";
            // the new model
            var employee = new EmployeeData
            {
                // here I call the variables outside of this new object that have one part of the split array called parts
                FirstName = firstName,
                LastName = lastName,
                DepartmentCode = deptCode,
                Salary = salaryResult,
                ManagerNum = numResult,
                PhoneNum = phoneNumber,
                Email = emailAddress,
                CreatedBy = createdBy,
                ModifiedBy = modifiedBy,
                StatusFlag = status,

                // Dates
                HireDate = hireDateResult,
                CreatedAt = createdOnResult,
                ModifiedAt = modifiedOnResult,
            };
            _cleanContext.Employees.Add(employee);
        }
        // 4. SaveChanges
        await _cleanContext.SaveChangesAsync();
    }
}