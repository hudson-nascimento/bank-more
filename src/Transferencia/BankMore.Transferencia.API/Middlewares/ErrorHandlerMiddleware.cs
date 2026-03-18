using BankMore.Transferencia.Domain.Errors;
using System.Net;
using System.Text.Json;

namespace BankMore.Transferencia.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                // Erro de negócio — 400, sem log de erro (é esperado)
                _logger.LogInformation(
                    "Erro de domínio: {Tipo} - {Mensagem}",
                    ex.TipoErro, ex.Message);

                await EscreverRespostaAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    ex.Message,
                    ex.TipoErro);
            }
            catch (UnauthorizedAccessException ex)
            {
                await EscreverRespostaAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    ex.Message,
                    TipoErro.NaoAutorizado);
            }
            catch (Exception ex)
            {
                // Erro inesperado — 500, log completo para investigação
                _logger.LogError(ex, "Erro inesperado na requisição");

                await EscreverRespostaAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Ocorreu um erro interno. Tente novamente.",
                    "INTERNAL_ERROR");
            }
        }

        private static async Task EscreverRespostaAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string mensagem,
            string tipo)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                mensagem,
                tipo
            });

            await context.Response.WriteAsync(body);
        }
    }
}