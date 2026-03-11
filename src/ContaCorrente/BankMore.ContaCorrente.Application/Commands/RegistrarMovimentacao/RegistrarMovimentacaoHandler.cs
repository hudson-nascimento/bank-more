using MediatR;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Errors;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Commands.RegistrarMovimentacao
{
    public class RegistrarMovimentacaoHandler
        : IRequestHandler<RegistrarMovimentacaoCommand>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public RegistrarMovimentacaoHandler(
            IContaCorrenteRepository contaRepository,
            IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task Handle(
            RegistrarMovimentacaoCommand request,
            CancellationToken cancellationToken)
        {            
            // Se já existe um movimento com esse IdRequisicao, retorna sem reprocessar
            var jaProcessado = await _movimentoRepository
                .ExisteMovimentoAsync(request.IdRequisicao);

            if (jaProcessado) return;

            var conta = await _contaRepository
                .ObterPorNumeroContaAsync(request.NumeroConta);

            // Validar existência da conta
            if (conta is null)
                throw new DomainException(
                    "Conta corrente não encontrada.",
                    TipoErro.ContaInvalida);

            // Validar se conta está ativa (regra na entidade)
            conta.ValidarMovimentacao();

            // Criar o movimento (validação de valor acontece no construtor)
            var movimento = new Movimento(
                request.IdRequisicao,
                request.NumeroConta,
                request.Valor,
                request.Tipo);

            // Persistir
            await _movimentoRepository.RegistrarAsync(movimento);
        }
    }
}