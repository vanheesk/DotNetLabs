var builder = DistributedApplication.CreateBuilder(args);

// ----- Exercise 4: Register the CatalogService -----
var catalogService = builder.AddProject<Projects.Lab_AspNet_10_CatalogService>("catalogservice");

// ----- Exercise 5: Wire up service discovery -----
var apiService = builder.AddProject<Projects.Lab_AspNet_10_ApiService>("apiservice")
    .WithReference(catalogService);

builder.Build().Run();
