using Dapper;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Enums;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DbConnectionFactory _factory;

        public MovimentoRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task RegistrarAsync(Movimento movimento)
        {
            using var connection = _factory.CreateConnection();

            var sql = @"
            INSERT INTO MOVIMENTO
                (IdRequisicao, NumeroConta, Valor, Tipo, DataHora)
            VALUES
                (@IdRequisicao, @NumeroConta, @Valor, @Tipo, @DataHora)";

            await connection.ExecuteAsync(sql, new
            {
                movimento.IdRequisicao,
                movimento.NumeroConta,
                movimento.Valor,
                Tipo = (int)movimento.Tipo,   
                DataHora = movimento.DataHora.ToString("O")
            });
        }

        public async Task<bool> ExisteMovimentoAsync(string idRequisicao)
        {
            using var connection = _factory.CreateConnection();

            var sql = @"
            SELECT COUNT(1)
            FROM MOVIMENTO
            WHERE IdRequisicao = @IdRequisicao";

            var count = await connection.ExecuteScalarAsync<int>(
                sql, new { IdRequisicao = idRequisicao });

            return count > 0;
        }

        public async Task<decimal> CalcularSaldoAsync(int numeroConta)
        {
            using var connection = _factory.CreateConnection();

            // Soma créditos, subtrai débitos
            var sql = @"
            SELECT COALESCE(
                SUM(CASE WHEN Tipo = @Credito THEN Valor
                         WHEN Tipo = @Debito  THEN -Valor
                    END), 0)
            FROM MOVIMENTO
            WHERE NumeroConta = @NumeroConta";

            return await connection.ExecuteScalarAsync<decimal>(sql, new
            {
                NumeroConta = numeroConta,
                Credito = (int)TipoMovimento.Credito,
                Debito = (int)TipoMovimento.Debito
            });
        }
    }
}