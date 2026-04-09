using DigitalSalaryService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using DigitalSalaryService.Persistence.Repositories.Abstract;

namespace DigitalSalaryService.Persistence.Repositories.Concrete
{
    public class BaseEFRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseEFRepository(DigitalSalaryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public async virtual Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstAsync(m => m.Id == id, cancellationToken);
        }

        public virtual IQueryable<T> GetAll(CancellationToken cancellationToken = default)
        {
            return _dbSet.AsQueryable<T>();
        }
        public async virtual Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
        public async virtual Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(cancellationToken);
        }
        public async virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(cancellationToken);
        }

        public async Task<bool> NotExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return !(await ExistsAsync(predicate, cancellationToken));
        }
    }
}
