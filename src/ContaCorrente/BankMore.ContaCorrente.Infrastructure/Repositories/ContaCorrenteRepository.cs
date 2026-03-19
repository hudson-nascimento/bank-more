using System.Security.Cryptography;
using System.Text;
using Dapper;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class ContaCorrenteRepository(DbConnectionFactory factory) : IContaCorrenteRepository
    {
        private readonly DbConnectionFactory _factory = factory;

        public async Task<int> CadastrarAsync(Domain.Entities.ContaCorrente conta)
        {
            using var connection = _factory.CreateConnection();

            // NumeroConta é gerado pelo AUTOINCREMENT do banco
            // Retornamos o último Id inserido como NumeroConta
            var sql = @"
            INSERT INTO CONTACORRENTE
                (NumeroConta, CpfHash, Cpf, SenhaHash, Ativo, DataCriacao)
            VALUES
                ((SELECT COALESCE(MAX(NumeroConta), 0) + 1 FROM CONTACORRENTE),
                 @CpfHash, @Cpf, @SenhaHash, 1, @DataCriacao);

            SELECT NumeroConta FROM CONTACORRENTE WHERE Id = last_insert_rowid();";

            var numeroConta = await connection.ExecuteScalarAsync<int>(sql, new
            {
                conta.CpfHash,
                Cpf = GerarHashBusca(conta.CpfHash), // SHA-256 do CPF original
                conta.SenhaHash,
                DataCriacao = conta.DataCriacao.ToString("O")
            });

            return numeroConta;
        }

        public async Task<Domain.Entities.ContaCorrente?> ObterPorNumeroContaAsync(int numeroConta)
        {
            using var connection = _factory.CreateConnection();

            var sql = @"
            SELECT Id, NumeroConta, CpfHash, SenhaHash,
                   Ativo, DataCriacao
            FROM CONTACORRENTE
            WHERE NumeroConta = @NumeroConta";

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.ContaCorrente>(
                sql, new { NumeroConta = numeroConta });
        }

        public async Task<Domain.Entities.ContaCorrente?> ObterPorCpfHashAsync(string cpf)
        {
            // Gerar o hash determinístico (SHA-256) do CPF para busca
            var Cpf = GerarHashBusca(cpf);

            using var connection = _factory.CreateConnection();

            var sql = @"
            SELECT Id, NumeroConta, CpfHash, SenhaHash,
                   Ativo, DataCriacao
            FROM CONTACORRENTE
            WHERE Cpf = @Cpf";

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.ContaCorrente>(
                sql, new { Cpf = Cpf });
        }

        public async Task InativarAsync(int numeroConta)
        {
            using var connection = _factory.CreateConnection();

            var sql = @"
            UPDATE CONTACORRENTE
            SET Ativo = 0
            WHERE NumeroConta = @NumeroConta";

            await connection.ExecuteAsync(sql, new { NumeroConta = numeroConta });
        }

        // Hash determinístico: mesmo CPF sempre gera o mesmo hash
        // Diferente do BCrypt (que é propositalmente não-determinístico)
        private static string GerarHashBusca(string valor)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}