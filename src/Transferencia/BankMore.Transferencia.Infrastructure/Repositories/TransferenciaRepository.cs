using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Repositories
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        public Task<bool> ExisteTransferenciaAsync(string idRequisicao)
        {
            throw new NotImplementedException();
        }

        public Task RegistrarAsync(Domain.Entities.Transferencia transferencia)
        {
            throw new NotImplementedException();
        }
    }
}
