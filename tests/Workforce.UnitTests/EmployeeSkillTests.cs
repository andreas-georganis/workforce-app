using Workforce.Domain.Model;

namespace Workforce.UnitTests;

public class EmployeeSkillTests
{
    [Fact]
    public void EmployeeSkill_Equals_ReturnsTrueForSameSkillId()
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

}