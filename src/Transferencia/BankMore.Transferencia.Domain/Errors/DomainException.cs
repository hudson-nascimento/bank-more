namespace BankMore.Transferencia.Domain.Errors
{
    public class DomainException : Exception
    {
        public string TipoErro { get; }

        public DomainException(string mensagem, string tipoErro)
            : base(mensagem)
        {
            TipoErro = tipoErro;
        }
    }
}