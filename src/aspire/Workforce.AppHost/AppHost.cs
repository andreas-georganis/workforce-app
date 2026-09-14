var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServerWithDatabase("WorkforceDb");

builder.AddDex();

var migration = builder.AddWorkforceApiMigration(db);

var api = builder.AddWorkforceApi(db, migration);

builder.AddWorkforceWeb(api);

await builder.Build().RunAsync();