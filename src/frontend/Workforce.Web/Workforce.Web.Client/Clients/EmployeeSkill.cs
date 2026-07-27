using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Workforce.Web.Client.Clients;

public sealed class EmployeeSkill
{
    [Required]
    public Guid? EmployeeId { get; set; }

    [Required]
    public Guid? SkillId { get; set; }

    [Required]
    public Proficiency Proficiency { get; set; } = Proficiency.Beginner;

    [Range(0, 100)]
    public int YearsOfExperience { get; set; }
}

/// <summary>
/// Proficiency values supported by the backend API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Proficiency
{
    Beginner,
    Intermediate,
    Expert
}
