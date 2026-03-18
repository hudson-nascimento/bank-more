using MediatR;

namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia
{
    public record EfetuarTransferenciaCommand(
        string IdRequisicao,
        int ContaDestino,
        decimal Valor,
        int ContaOrigem,   // vem do token JWT
        string Token       // repassado para as chamadas à API
    ) : IRequest;
}