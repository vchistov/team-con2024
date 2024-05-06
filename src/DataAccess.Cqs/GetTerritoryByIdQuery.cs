namespace TeamCon2024.DataAccess.Cqs;

using TeamCon2024.Core.DataAccess;
using TeamCon2024.DataAccess.Cqs.Abstractions;

public record GetTerritoryByIdQuery(string TerritoryId) : IQuery<TerritoryRecord?>;
