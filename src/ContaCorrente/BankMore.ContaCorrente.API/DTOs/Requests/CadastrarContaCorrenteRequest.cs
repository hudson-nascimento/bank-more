using System.ComponentModel.DataAnnotations;

namespace BankMore.ContaCorrente.API.DTOs.Requests
{
    public record CadastrarContaCorrenteRequest(
        [Required(ErrorMessage = "CPF é obrigatório")]
        string Cpf,

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        string Senha
    );
}