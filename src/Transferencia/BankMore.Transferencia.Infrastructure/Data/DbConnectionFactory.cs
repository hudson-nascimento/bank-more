using System.Data;
using Microsoft.Data.Sqlite;

namespace BankMore.Transferencia.Infrastructure.Data;

public class DbConnectionFactory(string connectionString)
{
    private readonly string _connectionString = connectionString;

    // Retorna IDbConnection. Para trocar para Oracle mudar aqui.
    // using Oracle.ManagedDataAccess.Client
    // OracleConnection(_connectionString);
    public IDbConnection CreateConnection()
    {
        return new
            SqliteConnection(_connectionString);
    }
}