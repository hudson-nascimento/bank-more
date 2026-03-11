using MediatR;
using BankMore.ContaCorrente.Domain.Enums;

namespace BankMore.ContaCorrente.Application.Commands.RegistrarMovimentacao
{
    public record RegistrarMovimentacaoCommand(
        string IdRequisicao,
        int NumeroConta,
        decimal Valor,
        TipoMovimento Tipo
    ) : IRequest;
}