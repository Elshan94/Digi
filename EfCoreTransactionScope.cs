using DigitalSalaryService.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalSalaryService.Persistance.Repositories
{
    internal class EfCoreTransactionScope : ITransactionScope, IDisposable, IAsyncDisposable
    {
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        public EfCoreTransactionScope(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _transaction.Dispose();
            }

            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await _transaction.DisposeAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EfCoreTransactionScope));
        }
    }
}
