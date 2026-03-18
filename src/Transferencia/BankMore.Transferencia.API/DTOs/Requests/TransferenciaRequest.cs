using System.ComponentModel.DataAnnotations;

namespace BankMore.Transferencia.API.DTOs.Requests
{
    public record TransferenciaRequest(
        [Required] string IdRequisicao,
        [Required] int ContaDestino,
        [Required] decimal Valor
    );
}