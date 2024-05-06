namespace TeamCon2024.DataAccess.Decorated;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.Core.Retries;

internal sealed class TerritoriesRepositoryDecorator : ITerritoriesRepository
{
    private readonly ITerritoriesRepository _innerRepository;
    private readonly IAsyncPolicy _retryPolicy;

    public TerritoriesRepositoryDecorator(ITerritoriesRepository innerRepository, ILogger<TerritoriesRepositoryDecorator> logger)
    {
        _innerRepository = innerRepository;
        _retryPolicy = PolicyFactory.CreateTransientDbRetryAsyncPolicy(RetrySettings.Default, logger);
    }

    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync(ct => _innerRepository.GetTerritoriesByRegionAsync(regionId, ct), cancellationToken);
    }

    public Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync(ct => _innerRepository.GetTerritoryByIdAsync(territoryId, ct), cancellationToken);
    }
}
