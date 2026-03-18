namespace BankMore.Transferencia.Domain.Interfaces
{
    public interface ITransferenciaRepository
    {
        Task RegistrarAsync(Entities.Transferencia transferencia);
        Task<bool> ExisteTransferenciaAsync(string idRequisicao);
    }
}