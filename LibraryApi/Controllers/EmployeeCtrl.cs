// Co-piot informed me of what the initial set up requirements are so that the script is compiled correctly.
// There was an error before but it needs a proper controller setup placeholder
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;  // ← Add this

[ApiController]                     // ← Add this
[Route("api/[controller]")]         // ← Add this
public class EmployeesController : ControllerBase
{
    // Here we add a constructor to build the context
    private readonly NewDbContext _context;

    public EmployeesController(NewDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeData>>> GetEmployees()
    {
        return await _context.Employees
            .Include(e => e.Manager)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeData>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
    
        if (employee == null)
        {
            return NotFound();
        }
    
        return employee;
    }

    // http post for creating new employee
    [HttpPost]
    public async Task<ActionResult<EmployeeData>> CreateEmployee(EmployeeData employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    // put for updating employee info
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeData employee)
    {
        if (id != employee.Id)
            return BadRequest();

        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
            return NotFound();

        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // delete employee by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
    
        if (employee == null)
            return NotFound();
    
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }
}

// do cd LibraryApi;   to get to the project folder before running run commands
// to run use - dotnet run

// to check employee data - http://localhost:5200/api/employees      -   use this link after running dotnet run