var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder
    .AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDbGate().ExcludeFromManifest()
    .WithAdminer().ExcludeFromManifest();

var db = sqlserver.AddDatabase("WorkforceDb");

var dexYaml = Path.Combine(AppContext.BaseDirectory, "dex-config.yaml");
var dex = builder
    .AddContainer("dex", "ghcr.io/dexidp/dex", "latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHttpEndpoint(
        name: "http",
        port: 5556,          // fixed host port
        targetPort: 5556,
        isProxied: false     // we want direct access for OIDC redirects
    )
    .WithEnvironment("DEX_SESSIONS_ENABLED", "true")
    .WithBindMount(dexYaml, "/etc/dex/config.docker.yaml")
    .WithArgs("dex", "serve", "/etc/dex/config.docker.yaml");

var api = builder.AddProject<Projects.Workforce_API>("workforce-api")
    .WithReference(db)
    .WithEnvironment("JWT__Authority", "http://127.0.0.1:5556/dex")
    .WithEnvironment("JWT__Audience", "workforce-app");

var migrator = builder.AddProject<Projects.Workforce_Migrator>("workforce-migrator")
    .WithReference(db)
    .WaitFor(db);

var migration = migrator.AddEFMigrations("api-migration")
    .WithMigrationsProject<Projects.Workforce_Migrator>()
    .WithReference(db)
    .WaitFor(db)
    .RunDatabaseUpdateOnStart()
    .PublishAsMigrationScript()
    .PublishAsMigrationBundle();

api.WaitForCompletion(migration);

var web = builder.AddProject<Projects.Workforce_Web>("workforce-web")
    // .WithHttpEndpoint(targetPort: 5000)
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints()
    .WithEnvironment("OIDC__Authority", "http://127.0.0.1:5556/dex")
    .WithEnvironment("OIDC__ClientId", "workforce-app")
    .WithEnvironment("OIDC__ClientSecret", "ZXhhbXBsZS1hcHAtc2VjcmV0");

builder.Build().Run();
