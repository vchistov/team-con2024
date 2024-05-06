namespace TeamCon2024.DataAccess.AllInOne;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Polly;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.Core.Retries;

internal sealed class TerritoriesRepository : ITerritoriesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _connectionString;
    private readonly IAsyncPolicy _retryPolicy;

    public TerritoriesRepository(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger<TerritoriesRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _connectionString = configuration.GetConnectionString("northwind") ?? throw new ArgumentException("Database connection string is absent.");
        _retryPolicy = PolicyFactory.CreateTransientDbRetryAsyncPolicy(RetrySettings.Default, logger);
    }

    public async Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async ct =>
        {
            using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(ct);

            var command = new CommandDefinition(
                "SELECT * FROM 'Territories' WHERE RegionID=@regionId",
                new { regionId },
                cancellationToken: ct);

            return (await connection.QueryAsync<TerritoryRecord>(command)).AsList();
        }, cancellationToken);
    }

    public Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken)
    {
        return ExecuteAsync(async ct =>
        {
            using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(ct);

            var command = new CommandDefinition(
                "SELECT * FROM 'Territories' WHERE TerritoryID=@territoryId",
                new { territoryId },
                cancellationToken: ct);

            return await connection.QueryFirstOrDefaultAsync<TerritoryRecord>(command);
        }, cancellationToken);
    }

    private Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync<TResult>(ct => action(ct), cancellationToken);
    }
}
