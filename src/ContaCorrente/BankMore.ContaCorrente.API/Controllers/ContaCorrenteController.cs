using BankMore.ContaCorrente.API.DTOs.Requests;
using BankMore.ContaCorrente.Application.Commands.CadastrarContaCorrente;
using BankMore.ContaCorrente.Application.Commands.EfetuarLogin;
using BankMore.ContaCorrente.Application.Commands.InativarContaCorrente;
using BankMore.ContaCorrente.Application.Commands.RegistrarMovimentacao;
using BankMore.ContaCorrente.Application.Queries.ConsultarSaldo;
using BankMore.ContaCorrente.Domain.Enums;
using BankMore.ContaCorrente.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.ContaCorrente.API.Controllers
{
    [ApiController]
    [Route("api/conta-corrente")]
    public class ContaCorrenteController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>Cadastra uma nova conta corrente</summary>
        /// <response code="200">Retorna o número da conta gerado</response>
        /// <response code="400">CPF inválido</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cadastrar(
            [FromBody] CadastrarContaCorrenteRequest request)
        {
            var command = new CadastrarContaCorrenteCommand(
                request.Cpf,
                request.Senha);

            var numeroConta = await _mediator.Send(command);

            return Ok(new { numeroConta });
        }

        /// <summary>Autentica e retorna token JWT</summary>
        /// <response code="200">Token JWT</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] EfetuarLoginRequest request)
        {
            var command = new EfetuarLoginCommand(
                request.NumeroConta,
                request.Cpf,
                request.Senha);

            var result = await _mediator.Send(command);

            return Ok(new { token = result.Token });
        }

        /// <summary>Inativa a conta corrente autenticada</summary>
        /// <response code="204">Conta inativada com sucesso</response>
        /// <response code="401">Senha inválida</response>
        /// <response code="403">Token inválido ou expirado</response>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Inativar(
            [FromBody] InativarContaCorrenteRequest request)
        {
            var numeroConta = ObterNumeroContaToken();

            var command = new InativarContaCorrenteCommand(
                numeroConta,
                request.Senha);

            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>Realiza débito ou crédito na conta corrente</summary>
        /// <response code="204">Movimentação registrada</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Token inválido ou expirado</response>
        [HttpPost("movimentacao")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Movimentar(
            [FromBody] MovimentacaoRequest request)
        {
            var numeroContaToken = ObterNumeroContaToken();

            var numeroConta = request.NumeroConta ?? numeroContaToken;

            // Regra: só pode creditar conta diferente — débito só na própria
            if (numeroConta != numeroContaToken && request.Tipo == "D")
                throw new DomainException(
                    "Débito só pode ser realizado na conta autenticada.",
                    TipoErro.TipoInvalido);

            var tipo = request.Tipo == "C"
                ? TipoMovimento.Credito
                : TipoMovimento.Debito;

            var command = new RegistrarMovimentacaoCommand(
                request.IdRequisicao,
                numeroConta,
                request.Valor,
                tipo);

            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>Retorna o saldo da conta autenticada</summary>
        /// <response code="200">Saldo atual com dados da conta</response>
        /// <response code="400">Conta inativa ou não encontrada</response>
        /// <response code="403">Token inválido ou expirado</response>
        [HttpGet("saldo")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConsultarSaldo()
        {
            var numeroConta = ObterNumeroContaToken();

            var query = new ConsultarSaldoQuery(numeroConta);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        private int ObterNumeroContaToken()
        {
            var claim = User.FindFirst("numeroConta")?.Value;

            if (string.IsNullOrEmpty(claim))
                throw new UnauthorizedAccessException("Token inválido.");

            return int.Parse(claim);
        }
    }
}