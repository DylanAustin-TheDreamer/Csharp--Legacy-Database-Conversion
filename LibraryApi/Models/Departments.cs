using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Departments
{
    [Key]
    [StringLength(10)]
    public string? DepartmentCode { get; set; }
    [StringLength(100)]
    public string? DepartmentName { get; set; }
    public decimal BudgetAmount { get; set; }
    public bool IsActive { get; set; }

    // need to reference the employee number
    public int DepartmentManagerNum { get; set; }
    [ForeignKey ("DepartmentManagerNum")]
    public EmployeeData? Manager { get; set; }
}