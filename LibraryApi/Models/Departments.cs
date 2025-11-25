using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Departments
{
    [Key]
    public string? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BudgetAmount { get; set; }
    public bool IsActive { get; set; }

    // need to reference the employee number
    public int DepartmentManagerNum { get; set; }
    [ForeignKey ("DepartmentManagerNum")]
    public EmployeeData? Manager { get; set; }
}