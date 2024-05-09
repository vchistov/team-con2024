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
[MedianColumn]
public class TerritoriesByIdBenchmarks
{
    private const string TerritoryId = "60601";

    private static readonly Dictionary<string, string> Settings = new(StringComparer.OrdinalIgnoreCase) { { "ConnectionStrings:northwind", "Data Source=northwind.db" } };

    private IServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;

    private DataAccess.AllInOne.ITerritoriesRepository _allInOneRepositoty;
    private DataAccess.Strategy.ITerritoriesRepository _strategyRepositoty;
    private DataAccess.Decorated.ITerritoriesRepository _decoratedRepositoty;
    private DataAccess.Cqs.Abstractions.IRepository _cqsRepositoty;
    private Repono.IRepository _sourceGenRepositoty;

    [GlobalSetup]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(Settings!)
            .Build();

        var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IConfiguration>(_ => configuration)
            .AddAllInOneDatabase()
            .AddStrategyDatabase()
            .AddDecoratedDatabase()
            .AddCqsDatabase()
            .AddSourceGeneratorsDatabase();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();

        _allInOneRepositoty = _serviceScope.ServiceProvider.GetRequiredService<DataAccess.AllInOne.ITerritoriesRepository>();
        _strategyRepositoty = _serviceScope.ServiceProvider.GetRequiredService<DataAccess.Strategy.ITerritoriesRepository>();
        _decoratedRepositoty = _serviceScope.ServiceProvider.GetRequiredService<DataAccess.Decorated.ITerritoriesRepository>();
        _cqsRepositoty = _serviceScope.ServiceProvider.GetRequiredService<DataAccess.Cqs.Abstractions.IRepository>();
        _sourceGenRepositoty = _serviceScope.ServiceProvider.GetRequiredService<Repono.IRepository>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceScope?.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
    }

    [Benchmark(Baseline = true)]
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
}
