using System.ComponentModel.DataAnnotations;

namespace BankMore.ContaCorrente.API.DTOs.Requests
{
    public record InativarContaCorrenteRequest(
        [Required(ErrorMessage = "Senha é obrigatória")]
        string Senha
    );
}