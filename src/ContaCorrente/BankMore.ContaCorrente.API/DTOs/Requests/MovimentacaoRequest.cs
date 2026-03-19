using System.ComponentModel.DataAnnotations;

namespace BankMore.ContaCorrente.API.DTOs.Requests
{
    public record MovimentacaoRequest(
        [Required(ErrorMessage = "IdRequisicao é obrigatório")]
        string IdRequisicao,

        // Se não vier, usa o do token
        int? NumeroConta,

        [Required]
        decimal Valor,

        // "C" ou "D", convertido para enum no Controller
        [Required]
        [RegularExpression("^[CD]$", ErrorMessage = "Tipo deve ser C ou D")]
        string Tipo
    );
}