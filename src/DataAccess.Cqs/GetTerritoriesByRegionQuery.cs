namespace TeamCon2024.DataAccess.Cqs;

using System.Collections.Generic;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.Cqs.Abstractions;

public record GetTerritoriesByRegionQuery(long RegionId) : IQuery<IReadOnlyCollection<TerritoryRecord>>;
