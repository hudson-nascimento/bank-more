using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.InativarContaCorrente
{
    public record InativarContaCorrenteCommand(
        int NumeroConta,  // token JWT
        string Senha
    ) : IRequest;
}