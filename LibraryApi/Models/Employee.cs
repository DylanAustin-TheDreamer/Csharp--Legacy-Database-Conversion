
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

public class EmployeeData
{
    [Key]
    public int Id { get; set; }          // Auto-property (getter + setter)

    [Required] // Not Null
    [StringLength(255)] // Max length
    public string? firstName { get; set; }   // ? means can be nullable
    public string? lastName { get; set; }
    public string? hireDate { get; set; }
}