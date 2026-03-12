using System.Data;
using Microsoft.Data.Sqlite;
using Oracle.ManagedDataAccess.Client;

namespace BankMore.ContaCorrente.Infrastructure.Data;

public class DbConnectionFactory(string connectionString)
{
    private readonly string _connectionString = connectionString;

    // Retorna IDbConnection. Para trocar para Oracle mudar aqui.
    // OracleConnection(_connectionString);
    public IDbConnection CreateConnection()
    {
        return new
            SqliteConnection(_connectionString);
    }
}