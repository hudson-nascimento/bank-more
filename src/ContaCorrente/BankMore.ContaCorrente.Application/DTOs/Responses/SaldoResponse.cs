namespace BankMore.ContaCorrente.Application.DTOs.Responses
{
    public record SaldoResponse(
        int NumeroConta,
        string NomeTitular,   // vem da conta corrente
        DateTime DataHoraConsulta,
        decimal Saldo
    );
}