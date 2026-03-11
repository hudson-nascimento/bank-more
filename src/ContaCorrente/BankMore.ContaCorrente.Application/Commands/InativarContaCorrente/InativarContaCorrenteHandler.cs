using MediatR;
using BankMore.ContaCorrente.Domain.Errors;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Commands.InativarContaCorrente
{
    public class InativarContaCorrenteHandler(IContaCorrenteRepository repository)
                : IRequestHandler<InativarContaCorrenteCommand>
    {
        private readonly IContaCorrenteRepository _repository = repository;

        public async Task Handle(
            InativarContaCorrenteCommand request,
            CancellationToken cancellationToken)
        {
            var conta = await _repository
                .ObterPorNumeroContaAsync(request.NumeroConta);

            if (conta is null)
                throw new DomainException(
                    "Conta corrente não encontrada.",
                    TipoErro.ContaInvalida);

            // Validar senha
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, conta.SenhaHash))
                throw new DomainException(
                    "Senha inválida.",
                    TipoErro.NaoAutorizado);

            // Regra de inativação na entidade
            conta.InativarConta();

            await _repository.InativarAsync(request.NumeroConta);
        }
    }
}