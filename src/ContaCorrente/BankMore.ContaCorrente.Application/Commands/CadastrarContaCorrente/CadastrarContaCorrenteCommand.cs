using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.CadastrarContaCorrente
{
    public record CadastrarContaCorrenteCommand(
        string Cpf,
        string Senha
    ) : IRequest<int>; // retorna o NumeroConta gerado
}