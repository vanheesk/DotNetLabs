var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Lab_AspNet_10_ApiService>("apiservice");

// TODO Exercise 4: Add a CatalogService project
// var catalogService = builder.AddProject<Projects.Lab_AspNet_10_CatalogService>("catalogservice");

// TODO Exercise 5: Wire up service discovery — tell ApiService about CatalogService
// apiService.WithReference(catalogService);

builder.Build().Run();
