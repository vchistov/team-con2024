namespace TeamCon2024.DataAccess.AllInOne;

using TeamCon2024.Core;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllInOneDatabase(this IServiceCollection services)
    {
        return services
            .AddDatabaseCore()
            .AddScoped<ITerritoriesRepository, TerritoriesRepository>();
    }
}
