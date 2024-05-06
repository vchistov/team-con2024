﻿namespace TeamCon2024.DataAccess.Cqs;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Polly;

using TeamCon2024.Core.Retries;
using TeamCon2024.DataAccess.Cqs.Abstractions;

internal sealed class TransientDbRetryQueryDecorator<TQuery, TResult> : IQueryDecorator<TQuery, TResult>
    where TQuery : notnull
{
    private readonly IAsyncPolicy _retryPolicy;

    public TransientDbRetryQueryDecorator(ILogger<TransientDbRetryQueryDecorator<TQuery, TResult>> logger)
    {
        _retryPolicy = PolicyFactory.CreateTransientDbRetryAsyncPolicy(RetrySettings.Default, logger);
    }

    public Task<TResult> ExecuteAsync(TQuery query, QueryHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        return _retryPolicy.ExecuteAsync(_ => next(), cancellationToken);
    }
}
