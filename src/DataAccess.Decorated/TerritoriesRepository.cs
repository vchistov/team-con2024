namespace TeamCon2024.DataAccess.Decorated;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TeamCon2024.Core.DataAccess;

using Dapper;

using Microsoft.Extensions.Configuration;

internal sealed class TerritoriesRepository : ITerritoriesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _connectionString;

    public TerritoriesRepository(IDbConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _connectionString = configuration.GetConnectionString("northwind") ?? throw new ArgumentException("Database connection string is absent.");
    }

    public async Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            "SELECT * FROM 'Territories' WHERE RegionID=@regionId",
            new { regionId },
            cancellationToken: cancellationToken);

        return (await connection.QueryAsync<TerritoryRecord>(command)).AsList();
    }

    public async Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            "SELECT * FROM 'Territories' WHERE TerritoryID=@territoryId",
            new { territoryId },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<TerritoryRecord>(command);
    }
}
