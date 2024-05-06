namespace TeamCon2024.DataAccess.DbClient;

using Microsoft.Extensions.DependencyInjection;

using TeamCon2024.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbClientDatabase(this IServiceCollection services)
    {
        return services
            .AddDatabaseCore()
            .AddSingleton<ITerritoriesRepository, TerritoriesRepository>();
    }
}
