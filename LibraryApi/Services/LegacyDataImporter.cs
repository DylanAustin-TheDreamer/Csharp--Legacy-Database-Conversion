using Microsoft.EntityFrameworkCore;
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
            var firstName = parts?[0];
            var lastName = parts?[1];
    
            // Then parse and create EmployeeData object
            var employee = new EmployeeData
            {
                // here I call the variables outside of this new object that have one part of the split array called parts
                FirstName = firstName,
                LastName = lastName,
            };
        }
        // 4. SaveChanges
        await _cleanContext.SaveChangesAsync();
    }
    
    private DateTime? ParseLegacyDate(string messyDate)
    {
        // Handle "01/15/2003", "2008-06-10", "Invalid Date"
        // TODO: Implement parsing logic
        return null; // Placeholder
    }
    
    private decimal? ParseLegacySalary(string messySalary)
    {
        // Handle "$95,000.00", "85000", "N/A"
        // TODO: Implement parsing logic
        return null; // Placeholder
    }
    
    // etc...
}