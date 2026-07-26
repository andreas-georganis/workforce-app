using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workforce.Migrator.Migrations;

/// <inheritdoc />
public partial class _20260726002122_Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateSequence(
            name: "employeeskillseq",
            incrementBy: 10);

        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                FirstName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Skills",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                Name = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Skills", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EmployeeSkill",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Proficiency = table.Column<int>(type: "int", nullable: false),
                YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmployeeSkill", x => x.Id);
                table.ForeignKey(
                    name: "FK_EmployeeSkill_Employees_EmployeeId",
                    column: x => x.EmployeeId,
                    principalTable: "Employees",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_EmployeeSkill_Skills_SkillId",
                    column: x => x.SkillId,
                    principalTable: "Skills",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Employees_Email",
            table: "Employees",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EmployeeSkill_EmployeeId_SkillId",
            table: "EmployeeSkill",
            columns: new[] { "EmployeeId", "SkillId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_EmployeeSkill_SkillId",
            table: "EmployeeSkill",
            column: "SkillId");

        migrationBuilder.CreateIndex(
            name: "IX_Skills_Name",
            table: "Skills",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EmployeeSkill");

        migrationBuilder.DropTable(
            name: "Employees");

        migrationBuilder.DropTable(
            name: "Skills");

        migrationBuilder.DropSequence(
            name: "employeeskillseq");
    }
}
