
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// we use [StringLength(num)] to specify the size for the database columns. if not in SQL we will get VARCHAR(MAX) which isn't necessary
// it would also impose performance issues
// we don't need to do that for other variable types - only strings in regards to database collumns
public class EmployeeData
{
    [Required]
    [Key]
    public int Id { get; set; }          // Auto-property (getter + setter)

    [StringLength(50)] // Max length
    public string? FirstName { get; set; }   // ? means can be nullable
    [StringLength(50)] // Max length
    public string? LastName { get; set; }

    [StringLength(10)] // Max length
    public string? DepartmentCode { get; set; }
    [ForeignKey("DepartmentCode")]  // ← Name of FK property
    public Departments? Department { get; set; }

    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }
    public bool StatusFlag {get; set; }

    public int? ManagerNum { get; set; }
    [ForeignKey("ManagerNum")]
    public EmployeeData? Manager { get; set; }
    [StringLength(200)] // Max length
    public string? Email { get; set; }
    [StringLength(16)] // Max length
    public string? PhoneNum { get; set; }
    [StringLength(200)] // Max length
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    [StringLength(200)] // Max length
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}