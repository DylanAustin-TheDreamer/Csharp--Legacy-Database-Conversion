using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
// Diagnostic: Log working directory and file status for Docker troubleshooting
var cwd = Directory.GetCurrentDirectory();
Console.WriteLine($"Working directory: {cwd}");
string[] filesToCheck = { "legacy_schema.sql", "legacy_sample_data.sql" };
foreach (var file in filesToCheck)
{
    var path = Path.Combine(cwd, file);
    Console.WriteLine($"File '{file}' exists: {File.Exists(path)}");
}
try
{
    var testFile = Path.Combine(cwd, "write_test.txt");
    File.WriteAllText(testFile, "test");
    Console.WriteLine($"Write test succeeded: {testFile}");
    File.Delete(testFile);
}
catch (Exception ex)
{
    Console.WriteLine($"Write test failed: {ex.Message}");
}

builder.Services.AddDbContext<NewDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "LegacyModernized.db")}"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
LegacyDatabaseBuilder.CreateLegacyDatabase();
builder.Services.AddTransient<LegacyDataImporter>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Import legacy data before running the app
using (var scope = app.Services.CreateScope())
{
    var cleanContext = scope.ServiceProvider.GetRequiredService<NewDbContext>();

    // Ensure database and tables are created
    cleanContext.Database.Migrate();

    // Break circular dependencies before deleting
    foreach (var emp in cleanContext.Employees)
    {
        emp.DepartmentCode = null;
        emp.ManagerNum = null;
    }
    foreach (var dept in cleanContext.Departments)
    {
        dept.DepartmentManagerNum = null;
    }
    cleanContext.SaveChanges();

    // Clear tables before import
    cleanContext.Employees.RemoveRange(cleanContext.Employees);
    cleanContext.Departments.RemoveRange(cleanContext.Departments);
    cleanContext.ProjectAssignments.RemoveRange(cleanContext.ProjectAssignments);
    cleanContext.SaveChanges();
    
    var importer = scope.ServiceProvider.GetRequiredService<LegacyDataImporter>();
    // First pass: import basic info
    importer.ImportAll().Wait();
    importer.ImportProjects().Wait();
    // Second pass: fix-up relationships
    importer.FixupEmployeeDepartmentsAndManagers().Wait();
    importer.FixupDepartmentManagers().Wait();
}

app.Run();