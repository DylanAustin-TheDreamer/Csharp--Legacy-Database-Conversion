using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers;

[ApiController]                     // ← Add this
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly NewDbContext _context;

    public ProjectController(NewDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectAssignTable>>> GetProjects()
    {
        return await _context.ProjectAssignments
            .Include(p => p.Employee) // or .Include(p => p.EmployeeData) if that's your property name
            .ToListAsync();
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<ProjectAssignTable>> GetProject(string code)
    {
        var project = await _context.ProjectAssignments.FindAsync(code);
    
        if (project == null)
        {
            return NotFound();
        }
    
        return project;
    }

    // http post for creating new department
    [HttpPost]
    public async Task<ActionResult<ProjectAssignTable>> CreateProject(ProjectAssignTable project)
    {
        _context.ProjectAssignments.Add(project);
        await _context.SaveChangesAsync();
    
        return CreatedAtAction(nameof(GetProject), new { code = project.AssignId }, project);
    }

    // put for updating department info
    [HttpPut("{code}")]
    public async Task<IActionResult> UpdateProject(string code, ProjectAssignTable project)
    {
        if (code != project.AssignId)
            return BadRequest();

        var existingProject = await _context.ProjectAssignments.FindAsync(code);
        if (existingProject == null)
            return NotFound();

        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // delete department by ID
    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteProject(string code)
    {
        var project = await _context.ProjectAssignments.FindAsync(code);
    
        if (project == null)
            return NotFound();
    
        _context.ProjectAssignments.Remove(project);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }
}