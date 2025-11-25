
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EmployeeData
{
    [Required]
    [Key]
    public int Id { get; set; }          // Auto-property (getter + setter)

    [Required] // Not Null
    [StringLength(255)] // Max length
    public string? FirstName { get; set; }   // ? means can be nullable
    public string? LastName { get; set; }

    [Required]
    [ForeignKey (nameof(DepartmentCode))]
    public string? DepartmentCode {get; set;}
    public Departments? departments { get; set; }

    public DateTime HireDate { get; set; }
    public decimal? Salary { get; set; }
    public bool StatusFlag {get; set; }
    public int ManagerNum { get; set; }
    public string? Email { get; set; }
    public string? PhoneNum { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
}