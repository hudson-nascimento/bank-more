using MediatR;
using BankMore.ContaCorrente.Application.DTOs.Responses;
using BankMore.ContaCorrente.Domain.Errors;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Queries.ConsultarSaldo
{
    public class ConsultarSaldoHandler
        : IRequestHandler<ConsultarSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ConsultarSaldoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<SaldoResponse> Handle(
            ConsultarSaldoQuery request,
            CancellationToken cancellationToken)
        {
            var conta = await _contaRepository
                .ObterPorNumeroContaAsync(request.NumeroConta);

            if (conta is null)
                throw new DomainException(
                    "Conta corrente não encontrada.",
                    TipoErro.ContaInvalida);

            if (!conta.Ativo)
                throw new DomainException(
                    "Conta corrente inativa.",
                    TipoErro.ContaInativa);

            // Saldo calculado no banco via SQL (soma créditos - débitos)
            var saldo = await _movimentoRepository.CalcularSaldoAsync(request.NumeroConta);

            return new SaldoResponse(
                NumeroConta: conta.NumeroConta,
                NomeTitular: "Titular",  // ContaCorrente não tem Nome ainda — ajuste conforme necessidade
                DataHoraConsulta: DateTime.UtcNow,
                Saldo: saldo
            );
        }
    }
}