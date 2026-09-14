public static class DistributedApplicationBuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        internal IResourceBuilder<SqlServerDatabaseResource> AddSqlServerWithDatabase(string databaseName)
        {
            var sqlserver = builder
            .AddSqlServer("sqlserver")
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDbGate().ExcludeFromManifest()
            .WithAdminer().ExcludeFromManifest();

            var db = sqlserver.AddDatabase(databaseName);

            return db;
        }

        internal IResourceBuilder<ContainerResource> AddDex()
        {
            var dexYaml = Path.Combine(AppContext.BaseDirectory, "dex-config.yaml");
            var clientSecret = builder.Configuration["OIDC:ClientSecret"]
                ?? throw new InvalidOperationException("OIDC:ClientSecret must be configured for Dex.");
            var dex = builder
                .AddContainer("dex", "ghcr.io/dexidp/dex")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithHttpEndpoint(
                    name: "http",
                    port: 5556,          // fixed host port
                    targetPort: 5556,
                    isProxied: false     // we want direct access for OIDC redirects
                )
                .WithEnvironment("DEX_SESSIONS_ENABLED", "true")
                .WithEnvironment("DEX_WORKFORCE_APP_CLIENT_SECRET", clientSecret)
                .WithBindMount(dexYaml, "/etc/dex/config.docker.yaml")
                .WithArgs("dex", "serve", "/etc/dex/config.docker.yaml");

            return dex;
        }

        internal IResourceBuilder<ProjectResource> AddWorkforceApi(IResourceBuilder<SqlServerDatabaseResource> db, IResourceBuilder<Aspire.Hosting.EntityFrameworkCore.EFMigrationResource> migration)
        {
            var api = builder.AddProject<Projects.Workforce_API>("workforce-api")
            .WithReference(db)
            .WithEnvironment("JWT__Authority", "http://127.0.0.1:5556/dex")
            .WithEnvironment("JWT__Audience", "workforce-app");

            api.WaitForCompletion(migration);

            return api;
        }

        internal IResourceBuilder<Aspire.Hosting.EntityFrameworkCore.EFMigrationResource> AddWorkforceApiMigration(IResourceBuilder<SqlServerDatabaseResource> db)
        {
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


            return migration;
        }

        internal IResourceBuilder<ProjectResource> AddWorkforceWeb(IResourceBuilder<ProjectResource> api)
        {
            var clientSecret = builder.Configuration["OIDC:ClientSecret"]
                ?? throw new InvalidOperationException("OIDC:ClientSecret must be configured for the web application.");
            var web = builder.AddProject<Projects.Workforce_Web>("workforce-web")
            //.WithHttpEndpoint(targetPort: 5000)
            .WithReference(api)
            .WaitFor(api)
            .WithExternalHttpEndpoints()
            .WithEnvironment("OIDC__Authority", "http://127.0.0.1:5556/dex")
            .WithEnvironment("OIDC__ClientId", "workforce-app")
            .WithEnvironment("OIDC__ClientSecret", clientSecret);

            return web;
        }
    }
}