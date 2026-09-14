using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Workforce.Domain.Model;

namespace Workforce.API.Contracts;

public sealed class EmployeeSkill
{
    [Required]
    public required EmployeeId EmployeeId {get; init;}

    [Required]
    public required SkillId SkillId {get; init;}

    [BindRequired]
    public required Proficiency Proficiency {get; init;}

    [BindRequired]
    public required YearsOfExperience YearsOfExperience {get; init;}
}