using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task RegistrarAsync(Movimento movimento);

        /// <summary>
        /// Verifica idempotência — se existe movimento com esse IdRequisicao
        /// </summary>
        /// <param name="idRequisicao"></param>
        /// <returns></returns>        
        Task<bool> ExisteMovimentoAsync(string idRequisicao);

        /// <summary>
        /// Soma créditos menos débitos para calcular saldo
        /// </summary>
        /// <param name="numeroConta"></param>
        /// <returns></returns>        
        Task<decimal> CalcularSaldoAsync(int numeroConta);
    }
}