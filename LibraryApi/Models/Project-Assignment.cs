using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectAssignTable
{
    [Required]
    [Key]
    public int AssignId { get; set; }

    // Get employee Id
    public int EmployeeNum { get; set; }
    [ForeignKey ("EmployeeNum")]
    public EmployeeData? Employee { get; set; }
    public string? ProjectCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int HrsPerWeek { get; set; }
    public decimal BillRate { get; set; }
    public string? Notes { get; set; }
}