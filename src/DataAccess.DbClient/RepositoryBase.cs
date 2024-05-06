namespace TeamCon2024.DataAccess.DbClient;

using System.Data.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Polly;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.Core.Retries;

internal abstract class RepositoryBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly IAsyncPolicy _retryPolicy;

    protected RepositoryBase(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _retryPolicy = PolicyFactory.CreateTransientDbRetryAsyncPolicy(RetrySettings.Default, logger);
    }

    protected virtual DbConnection CreateConnection(string name)
    {
        var connectionString = _configuration.GetConnectionString(name) ?? throw new ArgumentException("Database connection string is absent.");
        return _connectionFactory.CreateConnection(connectionString);
    }

    protected Task<TResult> ExecuteAsync<TResult>(Func<DbConnection, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        return ExecuteAsync("northwind", action, cancellationToken);
    }

    protected Task<TResult> ExecuteAsync<TResult>(string connectionName, Func<DbConnection, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync(async ct =>
        {
            using var connection = CreateConnection(connectionName);
            await connection.OpenAsync(ct);

            return await action(connection, ct);
        }, cancellationToken);
    }
}
