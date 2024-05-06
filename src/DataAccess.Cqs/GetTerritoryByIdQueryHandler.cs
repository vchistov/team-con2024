namespace TeamCon2024.DataAccess.Cqs;

using System;
using System.Threading;
using System.Threading.Tasks;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.Cqs.Abstractions;

using Dapper;

using Microsoft.Extensions.Configuration;

internal sealed class GetTerritoryByIdQueryHandler : IQueryHandler<GetTerritoryByIdQuery, TerritoryRecord?>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly string _connectionString;

    public GetTerritoryByIdQueryHandler(IDbConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _connectionString = configuration.GetConnectionString("northwind") ?? throw new ArgumentException("Database connection string is absent.");
    }

    public async Task<TerritoryRecord?> ExecuteAsync(GetTerritoryByIdQuery query, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(
            "SELECT * FROM 'Territories' WHERE TerritoryID=@territoryId",
            new { territoryId = query.TerritoryId },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<TerritoryRecord>(command);
    }
}
