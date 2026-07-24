using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Workforce.Domain.Model;

namespace Workforce.API.Contracts;

public sealed class EmployeeSkill
{
    [BindRequired]
    public required Proficiency Proficiency {get; init;}

    [Required]
    public required YearsOfExperience YearsOfExperience {get; init;}
}