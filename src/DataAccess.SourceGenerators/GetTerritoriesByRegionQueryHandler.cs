namespace TeamCon2024.DataAccess.SourceGenerators;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Configuration;

using Repono;

using TeamCon2024.Core.DataAccess;

internal sealed class GetTerritoriesByRegionQueryHandler : IQueryHandler<GetTerritoriesByRegionQuery, IReadOnlyCollection<TerritoryRecord>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _connectionString;

    public GetTerritoriesByRegionQueryHandler(IDbConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _connectionString = configuration.GetConnectionString("northwind") ?? throw new ArgumentException("Database connection string is absent.");
    }

    public async Task<IReadOnlyCollection<TerritoryRecord>> ExecuteAsync(GetTerritoriesByRegionQuery query, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            "SELECT * FROM 'Territories' WHERE RegionID=@regionId",
            new { regionId = query.RegionId },
            cancellationToken: cancellationToken);

        return (await connection.QueryAsync<TerritoryRecord>(command)).AsList();
    }
}
