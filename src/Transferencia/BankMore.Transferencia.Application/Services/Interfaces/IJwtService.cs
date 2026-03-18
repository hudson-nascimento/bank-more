namespace BankMore.Transferencia.Application.Services.Interfaces
{
    public interface IJwtService
    {
        string GerarToken(int numeroConta);
    }
}