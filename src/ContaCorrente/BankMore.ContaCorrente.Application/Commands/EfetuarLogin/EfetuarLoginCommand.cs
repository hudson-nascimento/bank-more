using MediatR;
using BankMore.ContaCorrente.Application.DTOs.Responses;

namespace BankMore.ContaCorrente.Application.Commands.EfetuarLogin
{
    public record EfetuarLoginCommand(
        string? NumeroConta,
        string? Cpf,
        string Senha
    ) : IRequest<LoginResponse>;
}