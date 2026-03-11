using MediatR;
using BankMore.ContaCorrente.Application.DTOs.Responses;

namespace BankMore.ContaCorrente.Application.Queries.ConsultarSaldo
{
    public record ConsultarSaldoQuery(int NumeroConta) : IRequest<SaldoResponse>;
}