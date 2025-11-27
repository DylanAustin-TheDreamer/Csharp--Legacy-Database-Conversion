using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]                     // ← Add this
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly NewDbContext _context;

    public DepartmentController(NewDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetDepartments()
    {
        var departments = await _context.Departments
            .Include(d => d.Manager) // or .Include(d => d.Manager) if that's your property name
            .Select(d => new
            {
               d.DepartmentCode,
               d.DepartmentName,
               d.DepartmentManagerNum,
               ManagerName = d.Manager != null ? d.Manager.FirstName + " " + d.Manager.LastName : null,
               d.BudgetAmount,
               d.IsActive
            })
            .ToListAsync();

        return departments;
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<object>> GetDepartment(string code)
    {
        var department = await _context.Departments
            .Include(d => d.Manager) // or .Include(d => d.Manager) if that's your property name
            .Select(d => new
            {
               d.DepartmentCode,
               d.DepartmentName,
               d.DepartmentManagerNum,
               ManagerName = d.Manager != null ? d.Manager.FirstName + " " + d.Manager.LastName : null,
               d.BudgetAmount,
               d.IsActive
            })
            .FirstOrDefaultAsync(d => d.DepartmentCode == code);
    
        if (department == null)
        {
            return NotFound();
        }
    
        return department;
    }

    // http post for creating new department
    [HttpPost]
    public async Task<ActionResult<Departments>> CreateDepartment(Departments department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
    
        return CreatedAtAction(nameof(GetDepartment), new { code = department.DepartmentCode }, department);
    }

    // put for updating department info
    [HttpPut("{code}")]
    public async Task<IActionResult> UpdateDepartment(string code, Departments department)
    {
        if (code != department.DepartmentCode)
            return BadRequest();

        var existingDepartment = await _context.Departments.FindAsync(code);
        if (existingDepartment == null)
            return NotFound();

        _context.Entry(department).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // delete department by ID
    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteDepartment(string code)
    {
        var department = await _context.Departments.FindAsync(code);
    
        if (department == null)
            return NotFound();
    
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }
}