namespace TeamCon2024.DataAccess.Cqs.Abstractions;

using System.Threading.Tasks;

public interface IQueryDecorator<TQuery, TResult>
    where TQuery : notnull
{
    Task<TResult> ExecuteAsync(TQuery query, QueryHandlerDelegate<TResult> next, CancellationToken cancellationToken);
}

public delegate Task<TResult> QueryHandlerDelegate<TResult>();
