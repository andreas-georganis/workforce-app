using System.ComponentModel.DataAnnotations;

namespace Workforce.Web.Client.Clients;

public sealed class Employee
{
	public Guid Id { get; set; }

	[Required]
	[StringLength(75, MinimumLength = 1)]
	public string FirstName { get; set; } = string.Empty;

	[Required]
	[StringLength(75, MinimumLength = 1)]
	public string LastName { get; set; } = string.Empty;

	[Required]
	[EmailAddress]
	[StringLength(254, MinimumLength = 3)]
	public string Email { get; set; } = string.Empty;
}
