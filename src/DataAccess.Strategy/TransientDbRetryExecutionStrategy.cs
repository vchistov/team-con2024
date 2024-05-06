namespace TeamCon2024.DataAccess.Strategy;

using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Polly;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.Core.Retries;

internal sealed class TransientDbRetryExecutionStrategy : IExecutionStrategy
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new(StringComparer.OrdinalIgnoreCase);

    private readonly IAsyncPolicy _retryPolicy;

    public TransientDbRetryExecutionStrategy(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger<TransientDbRetryExecutionStrategy> logger)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _retryPolicy = PolicyFactory.CreateTransientDbRetryAsyncPolicy(RetrySettings.Default, logger);
    }

    public Task<TResult> ExecuteAsync<TResult>(string connectionName, Func<DbConnection, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken)
    {
        var connectionString = GetConnectionString(connectionName);

        return _retryPolicy.ExecuteAsync(async ct =>
        {
            using var connection = _connectionFactory.CreateConnection(connectionString);
            await connection.OpenAsync(ct);

            return await action(connection, ct);
        }, cancellationToken);
    }

    private string GetConnectionString(string connectionName)
    {
        return _connectionStrings.GetOrAdd(
            connectionName,
            name => _configuration.GetConnectionString(name) ?? throw new ArgumentException("Database connection string is absent."));
    }
}
