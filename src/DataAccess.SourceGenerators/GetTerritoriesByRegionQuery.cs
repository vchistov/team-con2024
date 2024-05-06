namespace TeamCon2024.DataAccess.SourceGenerators;

using System.Collections.Generic;

using Repono;

using TeamCon2024.Core.DataAccess;

public record GetTerritoriesByRegionQuery(long RegionId) : IQuery<IReadOnlyCollection<TerritoryRecord>>;
