using BankMore.ContaCorrente.Domain.Errors;

namespace BankMore.ContaCorrente.Domain.Entities
{
    public class ContaCorrente
    {
        /// <summary>
        /// Código identificador da conta corrente
        /// </summary>
        public int Id { get; private set; }

        public int NumeroConta { get; private set; }

        public string CpfHash { get; private set; }

        public string SenhaHash { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime DataCriacao { get; private set; }

        // Construtor para criar nova conta
        public ContaCorrente(string cpfHash, string senhaHash)
        {
            CpfHash = cpfHash;
            SenhaHash = senhaHash;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
        }

        // Construtor para banco (Dapper)
        public ContaCorrente(
            int id,
            int numeroConta,
            string cpfHash,
            string senhaHash,
            bool ativo,
            DateTime dataCriacao)
        {
            Id = id;
            NumeroConta = numeroConta;
            CpfHash = cpfHash;
            SenhaHash = senhaHash;
            Ativo = ativo;
            DataCriacao = dataCriacao;
        }

        /// <summary>
        /// Valida se conta pode receber movimentação
        /// </summary>
        /// <exception cref="DomainException"></exception>
        public void ValidarMovimentacao()
        {
            if (!Ativo)
                throw new DomainException(
                    "A conta corrente está inativa.",
                    TipoErro.ContaInativa);
        }

        /// <summary>
        /// Inativar conta
        /// </summary>
        /// <exception cref="DomainException"></exception>
        public void InativarConta()
        {
            if (!Ativo)
                throw new DomainException(
                    "A conta corrente já está inativa.",
                    TipoErro.ContaInativa);

            Ativo = false;
        }
    }
}