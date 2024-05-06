namespace TeamCon2024.Core.DataAccess;

using System.Data.Common;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection(string connectionString);
}
