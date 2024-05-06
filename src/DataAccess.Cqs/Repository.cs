namespace TeamCon2024.DataAccess.Cqs;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.Cqs.Abstractions;

internal sealed class Repository : IRepository
{
    private readonly IServiceProvider _serviceProvider;

    public Repository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        if (query is GetTerritoriesByRegionQuery q1)
        {
            var task = ExecuteInternalAsync<GetTerritoriesByRegionQuery, IReadOnlyCollection<TerritoryRecord>>(q1, cancellationToken);
            return (task as Task<TResult>)!;
        }

        if (query is GetTerritoryByIdQuery q2)
        {
            var task = ExecuteInternalAsync<GetTerritoryByIdQuery, TerritoryRecord?>(q2, cancellationToken);
            return (task as Task<TResult>)!;
        }

        throw new InvalidOperationException($"Query handler not found for {query.GetType()}.");
    }

    private Task<TResult> ExecuteInternalAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : notnull, IQuery<TResult>
    {
        var decorators = _serviceProvider.GetServices<IQueryDecorator<TQuery, TResult>>() ?? Enumerable.Empty<IQueryDecorator<TQuery, TResult>>();
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        var pipeline = decorators.Aggregate(
            handler.ExecuteAsync,
            (h, d) => (q, ct) => d.ExecuteAsync(q, () => h(q, ct), ct));

        return pipeline.Invoke(query, cancellationToken);
    }
}
