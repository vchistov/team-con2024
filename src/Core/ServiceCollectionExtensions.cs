namespace TeamCon2024.Core;

using TeamCon2024.Core.DataAccess;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseCore(this IServiceCollection services)
    {
        return services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
    }
}
