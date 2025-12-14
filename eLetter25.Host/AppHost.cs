using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("eletter25db");

builder.AddProject<eLetter25_API>("letterAPI").WithReference(db);

builder.Build().Run();