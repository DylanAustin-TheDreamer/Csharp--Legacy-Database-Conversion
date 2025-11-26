using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeHireDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DepartmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BudgetAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DepartmentManagerNum = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentCode);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DepartmentCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Salary = table.Column<decimal>(type: "TEXT", nullable: true),
                    StatusFlag = table.Column<bool>(type: "INTEGER", nullable: false),
                    ManagerNum = table.Column<int>(type: "INTEGER", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhoneNum = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentCode",
                        column: x => x.DepartmentCode,
                        principalTable: "Departments",
                        principalColumn: "DepartmentCode");
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerNum",
                        column: x => x.ManagerNum,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectAssignments",
                columns: table => new
                {
                    AssignId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeNum = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HrsPerWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    BillRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAssignments", x => x.AssignId);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Employees_EmployeeNum",
                        column: x => x.EmployeeNum,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentManagerNum",
                table: "Departments",
                column: "DepartmentManagerNum");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentCode",
                table: "Employees",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerNum",
                table: "Employees",
                column: "ManagerNum");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_EmployeeNum",
                table: "ProjectAssignments",
                column: "EmployeeNum");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Employees_DepartmentManagerNum",
                table: "Departments",
                column: "DepartmentManagerNum",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Employees_DepartmentManagerNum",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "ProjectAssignments");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
