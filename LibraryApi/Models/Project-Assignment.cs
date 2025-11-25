using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ProjectAssignTable
{
    [Key]
    public int assignId { get; set; }

    [ForeignKey (nameof(employeeNum))]
    public int employeeNum { get; set; }
    public EmployeeData? employeeData { get; set; }
    public string? projectCode { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public int hrsPerWeek { get; set; }
    public decimal billRate { get; set; }
    public string? notes { get; set; }
}