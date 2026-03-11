using MediatR;
using BankMore.ContaCorrente.Application.DTOs.Responses;
using BankMore.ContaCorrente.Application.Services.Interfaces;
using BankMore.ContaCorrente.Domain.Errors;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Commands.EfetuarLogin
{
    public class EfetuarLoginHandler
        : IRequestHandler<EfetuarLoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _repository;
        private readonly IJwtService _jwtService;

        public EfetuarLoginHandler(
            IContaCorrenteRepository repository,
            IJwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(
            EfetuarLoginCommand request,
            CancellationToken cancellationToken)
        {
            // Buscar conta por número OU por CPF
            var conta = request.NumeroConta is not null
                ? await _repository.ObterPorNumeroContaAsync(int.Parse(request.NumeroConta))
                : await _repository.ObterPorCpfHashAsync(
                      request.Cpf!);
                        
            if (conta is null || !BCrypt.Net.BCrypt.Verify(request.Senha, conta.SenhaHash))
                throw new DomainException(
                    "Número de conta, CPF ou senha inválidos.",
                    TipoErro.NaoAutorizado);

                       var token = _jwtService.GerarToken(conta.NumeroConta);
            return new LoginResponse(token);
        }
    }
}