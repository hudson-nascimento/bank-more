namespace BankMore.ContaCorrente.Domain.Errors
{
    public class DomainException(string mensagem, string tipoErro) 
        : Exception(mensagem)
    {
        public string TipoErro { get; } = tipoErro;
    }
}