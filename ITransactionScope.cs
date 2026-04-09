namespace DigitalSalaryService.Persistence.Repositories.Abstract
{
    public interface ITransactionScope : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
