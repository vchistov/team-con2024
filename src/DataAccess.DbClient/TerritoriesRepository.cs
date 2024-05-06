namespace TeamCon2024.DataAccess.DbClient;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TeamCon2024.Core.DataAccess;

internal sealed class TerritoriesRepository : RepositoryBase, ITerritoriesRepository
{
    public TerritoriesRepository(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger<TerritoriesRepository> logger)
        : base(connectionFactory, configuration, logger)
    {
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

    public async Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async (connection, ct) =>
        {
            var command = new CommandDefinition(
                "SELECT * FROM 'Territories' WHERE TerritoryID=@territoryId",
                new { territoryId },
                cancellationToken: ct);

            return await connection.QueryFirstOrDefaultAsync<TerritoryRecord>(command);
        }, cancellationToken);
    }
}
