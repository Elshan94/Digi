using DigitalSalaryService.Persistance.Repositories;
using DigitalSalaryService.Persistence.Repositories.Abstract;
using Elasticsearch.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace DigitalSalaryService.Persistence.Repositories.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DigitalSalaryDbContext _digitalAppDbContext;
        private readonly IMediator _mediator;
        public UnitOfWork(DigitalSalaryDbContext digitalAppDbContext,
                                    IMediator mediator)
        {
            _digitalAppDbContext = digitalAppDbContext;
            _mediator = mediator;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _digitalAppDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ITransactionScope> BeginTransactionScopeAsync(CancellationToken cancellationToken = default)
        {
            var transactionScope = await _digitalAppDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            return new EfCoreTransactionScope(transactionScope);
        }

        public async Task CommitTransactionsAsync(ITransactionScope transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            try
            {
                await _digitalAppDbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionsAsync(transaction, cancellationToken);
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task RollbackTransactionsAsync(ITransactionScope transactionScope, CancellationToken cancellationToken = default)
        {
            if (transactionScope == null) throw new ArgumentNullException(nameof(transactionScope));

            try
            {
                await transactionScope.RollbackAsync(cancellationToken);
            }
            finally
            {
                transactionScope.Dispose();
            }
        }
    }
}
