namespace TeamCon2024.DataAccess.Decorated;

using Microsoft.Extensions.DependencyInjection;

using TeamCon2024.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDecoratedDatabase(this IServiceCollection services)
    {
        return services
            .AddDatabaseCore()
            .AddSingleton<TerritoriesRepository>()
            .AddSingleton<ITerritoriesRepository>(sp => ActivatorUtilities.CreateInstance<TerritoriesRepositoryDecorator>(sp, sp.GetRequiredService<TerritoriesRepository>()));
    }
}
