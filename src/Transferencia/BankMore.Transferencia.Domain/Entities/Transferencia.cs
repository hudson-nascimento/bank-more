namespace BankMore.Transferencia.Domain.Entities
{
    public class Transferencia
    {
        public int Id { get; private set; }
        public string IdRequisicao { get; private set; } // Idempotência
        public int ContaOrigem { get; private set; }
        public int ContaDestino { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataHora { get; private set; }

        public Transferencia(
            string idRequisicao,
            int contaOrigem,
            int contaDestino,
            decimal valor)
        {
            IdRequisicao = idRequisicao;
            ContaOrigem = contaOrigem;
            ContaDestino = contaDestino;
            Valor = valor;
            DataHora = DateTime.UtcNow;
        }
    }
}