namespace TeamCon2024.Benchmarks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.AllInOne;
using TeamCon2024.DataAccess.Cqs;
using TeamCon2024.DataAccess.Decorated;
using TeamCon2024.DataAccess.SourceGenerators;
using TeamCon2024.DataAccess.Strategy;

[MemoryDiagnoser]
public class Benchmarkie
{
    private const int RegionId = 3;
    private const string TerritoryId = "60601";

    private static readonly Dictionary<string, string> Settings = new(StringComparer.OrdinalIgnoreCase) { { "ConnectionStrings:northwind", "Data Source=northwind.db" } };

    private readonly DataAccess.AllInOne.ITerritoriesRepository _allInOneRepositoty;
    private readonly DataAccess.Strategy.ITerritoriesRepository _strategyRepositoty;
    private readonly DataAccess.Decorated.ITerritoriesRepository _decoratedRepositoty;
    private readonly DataAccess.Cqs.Abstractions.IRepository _cqsRepositoty;
    private readonly Repono.IRepository _sourceGenRepositoty;

    public Benchmarkie()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(Settings!)
            .Build();

        _allInOneRepositoty = GetAllInOneRepository(configuration);
        _strategyRepositoty = GetStrategyRepository(configuration);
        _decoratedRepositoty = GetDecoratedRepository(configuration);
        _cqsRepositoty = GetCqsRepository(configuration);
        _sourceGenRepositoty = GetSourceGenRepository(configuration);
    }

    [Benchmark]
    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegion_AllInOne()
    {
        return _allInOneRepositoty.GetTerritoriesByRegionAsync(RegionId, default);
    }

    [Benchmark]
    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegion_Strategy()
    {
        return _strategyRepositoty.GetTerritoriesByRegionAsync(RegionId, default);
    }

    [Benchmark]
    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegion_Decorated()
    {
        return _decoratedRepositoty.GetTerritoriesByRegionAsync(RegionId, default);
    }

    [Benchmark]
    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegion_Cqs()
    {
        return _cqsRepositoty.ExecuteAsync(new DataAccess.Cqs.GetTerritoriesByRegionQuery(RegionId), default);
    }

    [Benchmark]
    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegion_SourceGen()
    {
        return _sourceGenRepositoty.ExecuteAsync(new DataAccess.SourceGenerators.GetTerritoriesByRegionQuery(RegionId), default);
    }

    [Benchmark]
    public Task<TerritoryRecord?> GetTerritoryByIdAsync_AllInOne()
    {
        return _allInOneRepositoty.GetTerritoryByIdAsync(TerritoryId, default);
    }

    [Benchmark]
    public Task<TerritoryRecord?> GetTerritoryByIdAsync_Strategy()
    {
        return _strategyRepositoty.GetTerritoryByIdAsync(TerritoryId, default);
    }

    [Benchmark]
    public Task<TerritoryRecord?> GetTerritoryByIdAsync_Decorated()
    {
        return _decoratedRepositoty.GetTerritoryByIdAsync(TerritoryId, default);
    }

    [Benchmark]
    public Task<TerritoryRecord?> GetTerritoryByIdAsync_Cqs()
    {
        return _cqsRepositoty.ExecuteAsync(new DataAccess.Cqs.GetTerritoryByIdQuery(TerritoryId), default);
    }

    [Benchmark]
    public Task<TerritoryRecord?> GetTerritoryByIdAsync_SourceGen()
    {
        return _sourceGenRepositoty.ExecuteAsync(new DataAccess.SourceGenerators.GetTerritoryByIdQuery(TerritoryId), default);
    }

    private static DataAccess.AllInOne.ITerritoriesRepository GetAllInOneRepository(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddAllInOneDatabase();

        return serviceCollection.BuildServiceProvider().GetRequiredService<DataAccess.AllInOne.ITerritoriesRepository>();
    }

    private static DataAccess.Strategy.ITerritoriesRepository GetStrategyRepository(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddStrategyDatabase();

        return serviceCollection.BuildServiceProvider().GetRequiredService<DataAccess.Strategy.ITerritoriesRepository>();
    }

    private static DataAccess.Decorated.ITerritoriesRepository GetDecoratedRepository(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddDecoratedDatabase();

        return serviceCollection.BuildServiceProvider().GetRequiredService<DataAccess.Decorated.ITerritoriesRepository>();
    }

    private static DataAccess.Cqs.Abstractions.IRepository GetCqsRepository(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddCqsDatabase();

        return serviceCollection.BuildServiceProvider().GetRequiredService<DataAccess.Cqs.Abstractions.IRepository>();
    }

    private static Repono.IRepository GetSourceGenRepository(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddSourceGeneratorsDatabase();

        return serviceCollection.BuildServiceProvider().GetRequiredService<Repono.IRepository>();
    }
}
