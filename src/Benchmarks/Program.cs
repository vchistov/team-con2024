using BenchmarkDotNet.Running;

using TeamCon2024.Benchmarks;

BenchmarkRunner.Run<TerritoriesByRegionIdBenchmarks>();
BenchmarkRunner.Run<TerritoriesByIdBenchmarks>();

/*
var benchmarkie = new Benchmarkie();

Console.WriteLine("=== territories ===");
var territories = await benchmarkie.GetTerritoriesByRegion_AllInOne();
foreach (var territory in territories)
{
    Console.WriteLine("{0}\t{1}\t{2}", territory.RegionID, territory.TerritoryID, territory.TerritoryDescription);
}

Console.WriteLine();
Console.WriteLine("=== chicago ===");
var chicago = await benchmarkie.GetTerritoryByIdAsync_AllInOne();
Console.WriteLine("{0}\t{1}\t{2}", chicago?.RegionID, chicago?.TerritoryID, chicago?.TerritoryDescription);
*/
