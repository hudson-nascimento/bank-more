using BankMore.ContaCorrente.Domain.Enums;
using BankMore.ContaCorrente.Domain.Errors;

namespace BankMore.ContaCorrente.Domain.Entities
{
    public class Movimento
    {
        public int Id { get; private set; }

        // Chave de idempotência — vem do cliente, garante que a
        // mesma requisição não seja reprocessada 
        public string IdRequisicao { get; private set; }

        public int NumeroConta { get; private set; }

        public decimal Valor { get; private set; }

        public TipoMovimento Tipo { get; private set; }

        public DateTime DataHora { get; private set; }

        // Construtor para novo movimento
        public Movimento(
            string idRequisicao,
            int numeroConta,
            decimal valor,
            TipoMovimento tipo)
        {
            ValidarValor(valor);

            IdRequisicao = idRequisicao;
            NumeroConta = numeroConta;
            Valor = valor;
            Tipo = tipo;
            DataHora = DateTime.UtcNow;
        }

        // Construtor para banco (Dapper)
        public Movimento(
            int id,
            string idRequisicao,
            int numeroConta,
            decimal valor,
            TipoMovimento tipo,
            DateTime dataHora)
        {
            Id = id;
            IdRequisicao = idRequisicao;
            NumeroConta = numeroConta;
            Valor = valor;
            Tipo = tipo;
            DataHora = dataHora;
        }

        private static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new DomainException(
                    "O valor da movimentação deve ser positivo.",
                    TipoErro.ValorInvalido);
        }
    }
}