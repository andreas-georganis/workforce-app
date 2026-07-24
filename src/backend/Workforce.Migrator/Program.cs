using Microsoft.EntityFrameworkCore;
using Workforce.Infrastructure;
using Workforce.Migrator;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMigration<WorkforceDbContext>();

// builder.Services.AddOpenTelemetry()
//     .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddPooledDbContextFactory<WorkforceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WorkforceDb"), sqlOptions =>
        sqlOptions.MigrationsAssembly(typeof(IDbSeeder<>).Assembly)));

builder.EnrichSqlServerDbContext<WorkforceDbContext>();

var host = builder.Build();
host.Run();
