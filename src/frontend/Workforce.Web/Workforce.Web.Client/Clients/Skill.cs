using System.ComponentModel.DataAnnotations;

namespace Workforce.Web.Client.Clients;

public sealed class Skill
{
	public Guid Id { get; set; }

	[Required]
	[StringLength(75, MinimumLength = 1)]
	public string Name { get; set; } = string.Empty;
}
