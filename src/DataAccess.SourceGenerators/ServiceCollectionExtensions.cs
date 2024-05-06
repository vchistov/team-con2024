namespace TeamCon2024.DataAccess.SourceGenerators;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Repono;

using TeamCon2024.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSourceGeneratorsDatabase(this IServiceCollection services)
    {
        services.TryAddEnumerable(new ServiceDescriptor(typeof(IQueryDecorator<,>), typeof(TransientDbRetryQueryDecorator<,>), ServiceLifetime.Scoped));

        return services
            .AddDatabaseCore()
            .AddRepono();
    }
}
