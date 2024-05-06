namespace TeamCon2024.Core.DataAccess;

using System.Data.Common;

using Microsoft.Data.Sqlite;

internal sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    public DbConnection CreateConnection(string connectionString)
    {
        return new SqliteConnection(connectionString);
    }
}
