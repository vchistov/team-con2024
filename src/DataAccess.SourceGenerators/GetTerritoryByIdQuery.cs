namespace TeamCon2024.DataAccess.SourceGenerators;

using Repono;

using TeamCon2024.Core.DataAccess;

public record GetTerritoryByIdQuery(string TerritoryId) : IQuery<TerritoryRecord?>;
