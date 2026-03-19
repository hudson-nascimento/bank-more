namespace BankMore.ContaCorrente.API.DTOs.Requests
{
    public record EfetuarLoginRequest(
        string? NumeroConta,
        string? Cpf,
        string Senha
    );
}