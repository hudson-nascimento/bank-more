using Dapper;

namespace BankMore.ContaCorrente.Infrastructure.Data;

/// <summary>
/// Funcionalidade para inicializar o esquema do banco de dados, criando as tabelas necessárias, caso elas ainda não existam.
/// </summary>
/// <param name="factory">A fábrica usada para criar conexões de banco de dados para inicializar o esquema do banco de dados.</param>
public class DatabaseInitializer(DbConnectionFactory factory)
{
    private readonly DbConnectionFactory _factory = factory;

    public void Initialize()
    {
        using var connection = _factory.CreateConnection();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS CONTACORRENTE (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                NumeroConta INTEGER NOT NULL UNIQUE,
                CpfHash     TEXT    NOT NULL,
                Cpf         TEXT    NOT NULL,
                SenhaHash   TEXT    NOT NULL,
                Ativo       INTEGER NOT NULL DEFAULT 1,
                DataCriacao TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS MOVIMENTO (
                Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                IdRequisicao TEXT    NOT NULL UNIQUE,
                NumeroConta  INTEGER NOT NULL,
                Valor        REAL    NOT NULL,
                Tipo         INTEGER NOT NULL,
                DataHora     TEXT    NOT NULL,
                FOREIGN KEY (NumeroConta)
                    REFERENCES CONTACORRENTE(NumeroConta)
            );
        ");
    }
}