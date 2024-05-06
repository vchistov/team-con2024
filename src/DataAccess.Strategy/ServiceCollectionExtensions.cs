namespace TeamCon2024.DataAccess.Strategy;

using Microsoft.Extensions.DependencyInjection;

using TeamCon2024.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrategyDatabase(this IServiceCollection services)
    {
        return services
            .AddDatabaseCore()
            .AddSingleton<IExecutionStrategy, TransientDbRetryExecutionStrategy>()
            .AddSingleton<ITerritoriesRepository, TerritoriesRepository>();
    }
}
