using MediatR;
using BankMore.ContaCorrente.Domain.Errors;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Commands.CadastrarContaCorrente;

public class CadastrarContaCorrenteHandler(IContaCorrenteRepository repository)
        : IRequestHandler<CadastrarContaCorrenteCommand, int>
{
    private readonly IContaCorrenteRepository _repository = repository;

    public async Task<int> Handle(
        CadastrarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        if (!ValidarCpf(request.Cpf))
            throw new DomainException(
                "O CPF informado é inválido.",
                TipoErro.DocumentoInvalido);

        // Gerar hash do CPF
        var cpfHash = BCrypt.Net.BCrypt.HashPassword(request.Cpf);

        // Gerar hash da senha
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        // Criar entidade
        var conta = new Domain.Entities.ContaCorrente(cpfHash, senhaHash);

        // Persistir e retornar o número da conta gerado
        var numeroConta = await _repository.CadastrarAsync(conta);

        return numeroConta;
    }

    /// <summary>
    /// Validação do CPF pelo algoritmo dos dígitos verificadores
    /// </summary>
    /// <param name="cpf"></param>
    /// <returns></returns>
    private static bool ValidarCpf(string cpf)
    {
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false; // "111.111.111-11"

        int[] multiplicadores1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplicadores2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        var soma1 = cpf.Take(9)
                       .Select((d, i) => int.Parse(d.ToString()) * multiplicadores1[i])
                       .Sum();
        var resto1 = soma1 % 11;
        var digito1 = resto1 < 2 ? 0 : 11 - resto1;

        var soma2 = cpf.Take(10)
                       .Select((d, i) => int.Parse(d.ToString()) * multiplicadores2[i])
                       .Sum();
        var resto2 = soma2 % 11;
        var digito2 = resto2 < 2 ? 0 : 11 - resto2;

        return cpf[9] == digito1.ToString()[0]
            && cpf[10] == digito2.ToString()[0];
    }

}