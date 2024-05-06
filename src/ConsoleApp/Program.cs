using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Repono;

using TeamCon2024.DataAccess.SourceGenerators;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection()
    .AddLogging()
    .AddSingleton<IConfiguration>(_ => configuration)
    .AddSourceGeneratorsDatabase();

var serviceProvider = serviceCollection.BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<IRepository>();

Console.WriteLine("=== Northern territories ===");
var territories = await repository.ExecuteAsync(new GetTerritoriesByRegionQuery(3), default);
foreach (var territory in territories)
{
    Console.WriteLine("{0}\t{1}\t{2}", territory.RegionID, territory.TerritoryID, territory.TerritoryDescription);
}

Console.WriteLine();
Console.WriteLine("=== Chicago info ===");
var chicago = await repository.ExecuteAsync(new GetTerritoryByIdQuery("60601"), default);
if (chicago != null)
{
    Console.WriteLine("{0}\t{1}\t{2}", chicago.RegionID, chicago.TerritoryID, chicago.TerritoryDescription);
}

/*
var repository = serviceProvider.GetRequiredService<ITerritoriesRepository>();

Console.WriteLine("=== Northern territories ===");
var territories = await repository.GetTerritoriesByRegionAsync(3, default);
foreach (var territory in territories)
{
    Console.WriteLine("{0}\t{1}\t{2}", territory.RegionID, territory.TerritoryID, territory.TerritoryDescription);
}

Console.WriteLine();
Console.WriteLine("=== Chicago info ===");
var chicago = await repository.GetTerritoryByIdAsync("60601", default);
if (chicago != null)
{
    Console.WriteLine("{0}\t{1}\t{2}", chicago.RegionID, chicago.TerritoryID, chicago.TerritoryDescription);
}*/
