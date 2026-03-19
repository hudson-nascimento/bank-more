// Transferencia.Infrastructure/Services/ContaCorrenteService.cs
using System.Net.Http.Json;
using BankMore.Transferencia.Domain.Errors;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Services;

public class ContaCorrenteService : IContaCorrenteService
{
    private readonly HttpClient _httpClient;

    // HttpClient injetado via IHttpClientFactory (configurado no Program.cs)
    public ContaCorrenteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DebitarAsync(
        string idRequisicao, decimal valor, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/movimentacao",
            new
            {
                idRequisicao,
                valor,
                tipo = "D"   // Débito
            });

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new DomainException(
                $"Falha ao debitar conta: {erro}",
                TipoErro.FalhaMovimentacao);
        }
    }

    public async Task CreditarAsync(
        string idRequisicao, int contaDestino, decimal valor, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/movimentacao",
            new
            {
                idRequisicao,
                numeroConta = contaDestino,
                valor,
                tipo = "C"   // Crédito
            });

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new DomainException(
                $"Falha ao creditar conta destino: {erro}",
                TipoErro.FalhaMovimentacao);
        }
    }
}