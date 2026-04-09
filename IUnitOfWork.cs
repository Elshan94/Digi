using System.Transactions;

namespace DigitalSalaryService.Persistence.Repositories.Abstract
{
    public interface IUnitOfWork
    {
        Task<ITransactionScope> BeginTransactionScopeAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionsAsync(ITransactionScope transaction, CancellationToken cancellationToken = default);
        Task RollbackTransactionsAsync(ITransactionScope transactionScope, CancellationToken cancellationToken = default);
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
