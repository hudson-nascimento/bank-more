using MediatR;
using BankMore.Transferencia.Domain.Errors;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaHandler
        : IRequestHandler<EfetuarTransferenciaCommand>
    {
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IContaCorrenteService _contaCorrenteService;

        public EfetuarTransferenciaHandler(
            ITransferenciaRepository transferenciaRepository,
            IContaCorrenteService contaCorrenteService)
        {
            _transferenciaRepository = transferenciaRepository;
            _contaCorrenteService = contaCorrenteService;
        }

        public async Task Handle(
            EfetuarTransferenciaCommand request,
            CancellationToken cancellationToken)
        {
            // Idempotência
            if (await _transferenciaRepository.ExisteTransferenciaAsync(request.IdRequisicao))
                return;

            if (request.Valor <= 0)
                throw new DomainException("Valor inválido.", TipoErro.ValorInvalido);

            // Passo 1 — Débito na conta origem
            await _contaCorrenteService.DebitarAsync(
                request.IdRequisicao,
                request.Valor,
                request.Token);

            try
            {
                // Passo 2 — Crédito na conta destino
                await _contaCorrenteService.CreditarAsync(
                    request.IdRequisicao,
                    request.ContaDestino,
                    request.Valor,
                    request.Token);
            }
            catch
            {
                // Passo 3 — Estorno se o crédito falhou
                // IdRequisicao diferente para o estorno não ser bloqueado pela idempotência
                await _contaCorrenteService.DebitarAsync(
                    $"{request.IdRequisicao}-estorno",
                    request.Valor,
                    request.Token);

                throw; // propaga o erro para o Controller retornar 400
            }

            // Passo 4 — Persistir a transferência
            var transferencia = new Domain.Entities.Transferencia(
                request.IdRequisicao,
                request.ContaOrigem,
                request.ContaDestino,
                request.Valor);

            await _transferenciaRepository.RegistrarAsync(transferencia);
        }
    }
}