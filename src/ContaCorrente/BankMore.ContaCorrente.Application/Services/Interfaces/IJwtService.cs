namespace BankMore.ContaCorrente.Application.Services.Interfaces
{
    public interface IJwtService
    {
        string GerarToken(int numeroConta);
    }
}