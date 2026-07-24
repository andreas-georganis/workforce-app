var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder
    .AddSqlServer("sql-server")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate().ExcludeFromManifest()
    .WithAdminer().ExcludeFromManifest();

var db = sqlserver.AddDatabase("WorkforceDb");

var api = builder.AddProject<Projects.Workforce_API>("workforce-api")
    .WithReference(db);

// var migrator = builder.AddProject<Projects.Workforce_Migrator>("workforce-migrator")
//     .WithReference(db)
//     .WaitFor(db);

var migrator = api.AddEFMigrations("api-migrations").WithMigrationsProject<Projects.Workforce_Migrator>().RunDatabaseUpdateOnStart();

api.WaitForCompletion(migrator);

var _ = builder.AddProject<Projects.Workforce_Web>("workforce-web")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
