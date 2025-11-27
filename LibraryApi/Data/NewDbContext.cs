using Microsoft.EntityFrameworkCore;

public class NewDbContext : DbContext
{
     // This is called constructor chaining or base constructor call.
     // ApplicationDbContext inherits from DbContext
     // DbContext (the parent class) has its own constructor that needs these options
    public NewDbContext(DbContextOptions<NewDbContext> options)
        // : base(options) means "call the parent class constructor and pass it these options"
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Departments>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.DepartmentManagerNum)
            .OnDelete(DeleteBehavior.SetNull);
    }

    // A little bit like calling a component in Unity - but we give it a name and it has the get set functionality.
    public DbSet<EmployeeData> Employees { get; set; }
    public DbSet<Departments> Departments { get; set; }
    public DbSet<ProjectAssignTable> ProjectAssignments { get; set; }
}


// example of constructor in C#
/*
public class CustomCar : Car
{
    public CustomCar(Engine engine, Color color) 
        : base(engine)  // ← Pass engine to parent Car constructor
    {
        // Now do custom stuff with color
        this.Color = color;
    }
}
*/