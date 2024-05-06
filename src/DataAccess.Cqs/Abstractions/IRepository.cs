namespace TeamCon2024.DataAccess.Cqs.Abstractions;

public interface IRepository
{
    Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}
