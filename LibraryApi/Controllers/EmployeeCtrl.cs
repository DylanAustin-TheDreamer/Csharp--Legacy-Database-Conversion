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
        return await _context.Employees.ToListAsync();
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
}

// do cd LibraryApi;   to get to the project folder before running run commands
// to run use - dotnet run