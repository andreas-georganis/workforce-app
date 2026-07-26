
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Workforce.Domain.Model;

namespace Workforce.Infrastructure;

public class WorkforceDbContext(DbContextOptions<WorkforceDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(WorkforceDbContext).Assembly);
    }

}