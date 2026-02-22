using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var usersDb = builder.AddPostgres("Identity")
    .WithHostPort(5484)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("users-db");

var persistence = builder.AddPostgres("Persistence")
    .WithHostPort(5583)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var appDb = persistence.AddDatabase("eletter25-db");
var hangfireDb = persistence.AddDatabase("hangfire-db");

var letterApi = builder.AddProject<eLetter25_API>("API")
    .WithReference(usersDb)
    .WithReference(appDb)
    .WithReference(hangfireDb)
    .WaitFor(appDb)
    .WaitFor(hangfireDb)
    .WaitFor(usersDb);

var client = builder.AddNpmApp("Client", Path.Combine("..", "eLetter25.Client", "eLetter25.Client"), "start")
    .WithReference(letterApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();