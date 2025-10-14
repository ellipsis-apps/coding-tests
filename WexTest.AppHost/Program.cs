var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.WexTest_ApiService>("apiservice");

builder.AddProject<Projects.WexTest_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
