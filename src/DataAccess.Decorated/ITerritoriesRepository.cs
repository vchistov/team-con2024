namespace TeamCon2024.DataAccess.Decorated;

using TeamCon2024.Core.DataAccess;

public interface ITerritoriesRepository
{
    Task<IReadOnlyCollection<TerritoryRecord>> GetTerritoriesByRegionAsync(long regionId, CancellationToken cancellationToken);

    Task<TerritoryRecord?> GetTerritoryByIdAsync(string territoryId, CancellationToken cancellationToken);
}
