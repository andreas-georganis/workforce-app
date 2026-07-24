using Microsoft.EntityFrameworkCore;

namespace Workforce.Migrator;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context, CancellationToken cancellationToken = default);
}