using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NewDbContext>(options =>
    options.UseSqlite("Data Source=LegacyModernized.db"));
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

    // Clear tables before import
    
    cleanContext.Employees.RemoveRange(cleanContext.Employees);
    cleanContext.Departments.RemoveRange(cleanContext.Departments);
    cleanContext.ProjectAssignments.RemoveRange(cleanContext.ProjectAssignments);
    cleanContext.SaveChanges();
    
    var importer = scope.ServiceProvider.GetRequiredService<LegacyDataImporter>();
    importer.ImportDepartments().Wait();
    importer.ImportEmployees().Wait();
    importer.ImportProjects().Wait();
    importer.CheckDepartmentManager().Wait();
}

app.Run();