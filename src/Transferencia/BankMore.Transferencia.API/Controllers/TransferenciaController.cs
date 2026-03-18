using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BankMore.Transferencia.API.DTOs.Requests;
using BankMore.Transferencia.Application.Commands.EfetuarTransferencia;

namespace BankMore.Transferencia.API.Controllers;

[ApiController]
[Route("api/transferencia")]
public class TransferenciaController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>Realiza transferência entre contas da mesma instituição</summary>
    /// <response code="204">Transferência realizada com sucesso</response>
    /// <response code="400">Dados inválidos ou falha na operação</response>
    /// <response code="403">Token inválido ou expirado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Transferir(
        [FromBody] TransferenciaRequest request)
    {
        // Extrair número da conta e token bruto do JWT
        var numeroConta = int.Parse(
            User.FindFirst("numeroConta")!.Value);

        // Token bruto para repassar à API de Conta Corrente
        var tokenBruto = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        var command = new EfetuarTransferenciaCommand(
            request.IdRequisicao,
            request.ContaDestino,
            request.Valor,
            numeroConta,
            tokenBruto);

        await _mediator.Send(command);

        return NoContent();
    }
}