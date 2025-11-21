# 📚 C# API Learning Guide

## Key Concepts You'll Learn

### 1. C# Classes and Properties

**What are they?**
- Classes define the structure of your data (like Django models)
- Properties are like Python class attributes but with more control

**Example:**
```csharp
public class Employee
{
    public int Id { get; set; }          // Auto-property (getter + setter)
    public string FirstName { get; set; }
}
```

**Compare to Python/Django:**
```python
class Employee(models.Model):
    id = models.IntegerField(primary_key=True)
    first_name = models.CharField(max_length=100)
```

---

### 2. Data Types in C#

| C# Type | Python/Django Equivalent | Used For |
|---------|-------------------------|----------|
| `int` | `int` | Whole numbers (IDs, counts) |
| `string` | `str` | Text |
| `decimal` | `Decimal` | Money, precise numbers |
| `DateTime` | `datetime` | Dates and times |
| `bool` | `bool` | True/False |
| `double` | `float` | Floating point numbers |

**Nullable Types:**
```csharp
int? nullableNumber;      // Can be null
DateTime? optionalDate;   // Can be null
```

The `?` means "this can be null" - like `null=True` in Django.

---

### 3. Entity Framework (The ORM)

**Like Django ORM, but C# style**

**Django:**
```python
employees = Employee.objects.all()
employee = Employee.objects.get(id=5)
Employee.objects.create(first_name="John")
```

**Entity Framework:**
```csharp
var employees = await _context.Employees.ToListAsync();
var employee = await _context.Employees.FindAsync(5);
_context.Employees.Add(new Employee { FirstName = "John" });
await _context.SaveChangesAsync();
```

---

### 4. Async/Await (Non-blocking code)

**Why?** Database calls are slow - don't block the server!

```csharp
// ❌ BAD - Blocks thread
var employees = _context.Employees.ToList();

// ✅ GOOD - Non-blocking
var employees = await _context.Employees.ToListAsync();
```

**Rule:** If method has `async`, it should return `Task` or `Task<T>`

```csharp
public async Task<ActionResult<Employee>> GetEmployee(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    return employee;
}
```

---

### 5. Attributes (Decorators in Python)

**Python decorators:**
```python
@api_view(['GET'])
def get_employees(request):
    pass
```

**C# attributes:**
```csharp
[HttpGet]
public async Task<IActionResult> GetEmployees()
{
    // ...
}
```

**Common API attributes:**
- `[ApiController]` - Marks class as API controller
- `[Route("api/[controller]")]` - Sets base URL
- `[HttpGet]` - GET request
- `[HttpPost]` - POST request
- `[HttpPut]` - PUT request
- `[HttpDelete]` - DELETE request

---

### 6. Dependency Injection (Magic Constructor)

**ASP.NET automatically provides services you need:**

```csharp
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    // ASP.NET sees you need DbContext and gives it to you!
    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

**Like Django's `self.request` but more explicit.**

---

### 7. ActionResult (Response Types)

**Django:**
```python
return JsonResponse({'name': 'John'})
return HttpResponse(status=404)
return Response(data, status=201)
```

**C#:**
```csharp
return Ok(employee);                    // 200 OK
return NotFound();                      // 404 Not Found
return BadRequest("Invalid data");      // 400 Bad Request
return CreatedAtAction(...);            // 201 Created
return NoContent();                     // 204 No Content
```

---

### 8. LINQ (Querying Data)

**Like Django QuerySet methods:**

**Django:**
```python
Employee.objects.filter(department='IT')
Employee.objects.filter(salary__gte=80000)
Employee.objects.filter(is_active=True).order_by('last_name')
```

**C# LINQ:**
```csharp
_context.Employees.Where(e => e.Department == "IT")
_context.Employees.Where(e => e.Salary >= 80000)
_context.Employees.Where(e => e.IsActive).OrderBy(e => e.LastName)
```

**Lambda expressions:** `e => e.Salary` means "take employee e, return e.Salary"

---

### 9. Migrations (Database Schema Changes)

**Django:**
```bash
python manage.py makemigrations
python manage.py migrate
```

**Entity Framework:**
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

Same concept, different syntax!

---

### 10. JSON Serialization (Automatic!)

**Django:** Need serializers (DRF)  
**ASP.NET:** Automatically converts objects to JSON!

```csharp
return Ok(employee);  // ← Automatically becomes JSON!
```

---

## Common Patterns

### GET All Items
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
{
    return await _context.Employees.ToListAsync();
}
```

### GET Single Item
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Employee>> GetEmployee(int id)
{
    var employee = await _context.Employees.FindAsync(id);
    
    if (employee == null)
        return NotFound();
    
    return employee;
}
```

### POST (Create)
```csharp
[HttpPost]
public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
{
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    return CreatedAtAction(nameof(GetEmployee), 
                          new { id = employee.Id }, 
                          employee);
}
```

### PUT (Update)
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
{
    if (id != employee.Id)
        return BadRequest();
    
    _context.Entry(employee).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    
    return NoContent();
}
```

### DELETE
```csharp
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
```

---

## File Structure

```
LibraryApi/
├── Models/              ← Your entity classes (like Django models.py)
│   └── Employee.cs
├── Data/                ← Database context (like Django database config)
│   └── ApplicationDbContext.cs
├── Controllers/         ← API endpoints (like Django views.py)
│   └── EmployeesController.cs
├── Program.cs           ← App configuration (like Django settings.py + wsgi.py)
├── appsettings.json     ← Config file (like Django settings.py for connection strings)
└── LibraryApi.csproj    ← Project file (like requirements.txt + more)
```

---

## Helpful Commands

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Create migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Add package
dotnet add package PackageName
```

---

## When to Ask for Help

✅ **Good questions:**
- "How do I make a property nullable?"
- "What's the difference between async and sync?"
- "How do I add a foreign key relationship?"
- "Why am I getting this error: [paste error]?"

❌ **Don't ask me to:**
- Write entire classes for you
- Solve logic problems without showing your attempt
- Debug without showing your code

---

## Debugging Tips

**Read the error message!** C# errors are verbose but helpful:

```
Cannot convert type 'string' to 'int'
```
→ You're trying to assign a string to an int variable

```
Object reference not set to an instance of an object
```
→ You're accessing a null object (null pointer exception)

```
InvalidOperationException: Sequence contains no elements
```
→ You're calling `.First()` on empty collection, use `.FirstOrDefault()`

---

## Ready to Code?

Start by creating your first model in `Models/Employee.cs`

Ask questions as you go! 🚀
