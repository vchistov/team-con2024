namespace TeamCon2024.DataAccess.Cqs.Abstractions;
public interface IQueryHandler<TQuery, TResult>
    where TQuery : notnull, IQuery<TResult>
{
    Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
}
