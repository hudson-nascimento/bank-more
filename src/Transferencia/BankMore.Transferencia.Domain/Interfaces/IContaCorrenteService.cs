namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Contrato para a chamada HTTP à API Conta Corrente
/// </summary>
public interface IContaCorrenteService
{
    Task DebitarAsync(string idRequisicao, decimal valor, string token);
    Task CreditarAsync(string idRequisicao, int contaDestino, decimal valor, string token);
}