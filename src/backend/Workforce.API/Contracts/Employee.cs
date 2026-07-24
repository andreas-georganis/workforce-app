using System.ComponentModel.DataAnnotations;
using Workforce.Domain.Model;

namespace Workforce.API.Contracts;

public sealed class Employee
{
    public EmployeeId Id { get; init; } = EmployeeId.New();

    [Required]
    public required FirstName FirstName { get; init; }

    [Required]
    public required LastName LastName { get; init; }

    [Required]
    public required Email Email { get; init; }
}