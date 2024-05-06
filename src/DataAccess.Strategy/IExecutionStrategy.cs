namespace TeamCon2024.DataAccess.Strategy;

using System;
using System.Data.Common;
using System.Threading.Tasks;

internal interface IExecutionStrategy
{
    Task<TResult> ExecuteAsync<TResult>(string connectionName, Func<DbConnection, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken);
}
