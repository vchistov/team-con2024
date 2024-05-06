namespace TeamCon2024.DataAccess.Strategy;

using TeamCon2024.Core.DataAccess;

public interface ITerritoriesRepository
{
    Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken);

    Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken);
}
