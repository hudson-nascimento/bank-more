namespace BankMore.ContaCorrente.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        /// <summary>
        /// Persiste a conta e retorna o NumeroConta gerado
        /// </summary>
        /// <param name="conta"></param>
        /// <returns></returns>
        Task<int> CadastrarAsync(Entities.ContaCorrente conta);

        /// <summary>
        /// Busca por número da conta — usado para login e movimentação
        /// </summary>
        /// <param name="numeroConta"></param>
        /// <returns></returns>        
        Task<Entities.ContaCorrente?> ObterPorNumeroContaAsync(int numeroConta);

        /// <summary>
        /// Busca por hash do CPF — usado no login por CPF
        /// </summary>
        /// <param name="cpfHash"></param>
        /// <returns></returns>
        Task<Entities.ContaCorrente?> ObterPorCpfHashAsync(string cpfHash);

        /// <summary>
        /// Atualiza o campo Ativo para 0 (inativação)
        /// </summary>
        /// <param name="numeroConta"></param>
        /// <returns></returns>        
        Task InativarAsync(int numeroConta);
    }
}

