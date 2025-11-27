using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectAssignTable
{
    [Required]
    [Key]
    public string? AssignId { get; set; }

    // Get employee Id
    public int? EmployeeNum { get; set; }
    [ForeignKey ("EmployeeNum")]
    public EmployeeData? Employee { get; set; }
    [StringLength(100)]
    public string? ProjectCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? HrsPerWeek { get; set; }
    public decimal? BillRate { get; set; }
    // leave this as VARCHAR(MAX)
    public string? Notes { get; set; }
}