namespace Workforce.UnitTests;

using Workforce.Domain.Exceptions;
using Workforce.Domain.Model;

public class EmployeeTests
{
    [Fact]
    public void Equals_ReturnsTrue()
    {
        // Arrange
        var skillId = SkillId.New();
        var proficiency = Proficiency.Intermediate;
        var yearsOfExperience = YearsOfExperience.New(3);

        var employeeSkill1 = new EmployeeSkill(skillId, proficiency, yearsOfExperience);
        var employeeSkill2 = new EmployeeSkill(skillId, proficiency, yearsOfExperience);

        // Act
        var result = employeeSkill1.Equals(employeeSkill2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Duplicate_Skill_Throws()
    {
        // Arrange
        var employeeId = EmployeeId.New();
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");
        var email = new Email("john.doe@example.com");

        var skillId = SkillId.New();
        var proficiency = Proficiency.Intermediate;
        var yearsOfExperience = YearsOfExperience.New(3);

        var employee = new Employee(employeeId, firstName, lastName, email, [new EmployeeSkill(skillId, proficiency, yearsOfExperience)]);

        // Act & Assert
        Assert.Throws<WorkforceDomainException>(() => employee.AssignSkill(skillId, proficiency, yearsOfExperience));
    }

    [Fact]
    public void Assign_Skill_Succeeds()
    {
        // Arrange
        var employeeId = EmployeeId.New();
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");
        var email = new Email("john.doe@example.com");

        var skillId = SkillId.New();
        var proficiency = Proficiency.Intermediate;
        var yearsOfExperience = YearsOfExperience.New(3);

        var employee = new Employee(employeeId, firstName, lastName, email, []);

        // Act
        employee.AssignSkill(skillId, proficiency, yearsOfExperience);

        // Assert
        Assert.Contains(employee.Skills, es => es.SkillId == skillId);
    }
}
