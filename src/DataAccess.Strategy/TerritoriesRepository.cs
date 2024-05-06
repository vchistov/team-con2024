namespace TeamCon2024.DataAccess.Strategy;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using TeamCon2024.Core.DataAccess;

internal sealed class TerritoriesRepository : ITerritoriesRepository
{
    private readonly IExecutionStrategy _executionStrategy;

    public TerritoriesRepository(IExecutionStrategy executionStrategy)
    {
        _executionStrategy = executionStrategy;
    }

    public Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken)
    {
        return ExecuteAsync<IReadOnlyCollection<TerritoryRecord>>(async (connection, ct) =>
        {
            var command = new CommandDefinition(
                "SELECT * FROM 'Territories' WHERE RegionID=@regionId",
                new { regionId },
                cancellationToken: ct);

            return (await connection.QueryAsync<TerritoryRecord>(command)).AsList();
        }, cancellationToken);
    }

    public Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken)
    {
        return ExecuteAsync((connection, ct) =>
        {
            var command = new CommandDefinition(
                "SELECT * FROM 'Territories' WHERE TerritoryID=@territoryId",
                new { territoryId },
                cancellationToken: ct);

            return connection.QueryFirstOrDefaultAsync<TerritoryRecord>(command);
        }, cancellationToken);
    }

    private Task<TResult> ExecuteAsync<TResult>(Func<DbConnection, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        return _executionStrategy.ExecuteAsync("northwind", action, cancellationToken);
    }
}
