namespace TeamCon2024.DataAccess.Cqs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TeamCon2024.Core;
using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.Cqs.Abstractions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCqsDatabase(this IServiceCollection services)
    {
        services.TryAddEnumerable(new ServiceDescriptor(typeof(IQueryDecorator<,>), typeof(TransientDbRetryQueryDecorator<,>), ServiceLifetime.Scoped));

        return services
            .AddDatabaseCore()
            .AddSingleton<IRepository, Repository>()
            .AddScoped<IQueryHandler<GetTerritoriesByRegionQuery, IReadOnlyCollection<TerritoryRecord>>, GetTerritoriesByRegionQueryHandler>()
            .AddScoped<IQueryHandler<GetTerritoryByIdQuery, TerritoryRecord?>, GetTerritoryByIdQueryHandler>();
    }
}
