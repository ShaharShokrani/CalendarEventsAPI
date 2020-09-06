using CalendarEvents.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{
    public interface IGenericRepository<TEntity> : IGetRepository<TEntity>, 
                                    IInsertRepository<TEntity>,
                                    IUpdateRepository<TEntity>,
                                    IRemoveRepository<TEntity>,
                                    IOwnerRepository<TEntity>,
                                    IDisposable where TEntity : class, IGenericEntity
    {
        Task<int> SaveChanges();
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IGenericEntity
    {
        private readonly ApplicationDbContext _context = null;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = this._context.Set<TEntity>();
        }

        public virtual async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
        //TODO: Add paging support
        public virtual IAsyncEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = this._dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                var properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var includeProperty in properties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).AsAsyncEnumerable();
            }

            return query.AsAsyncEnumerable();
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {            
            return await _dbSet.FirstOrDefaultAsync(item => item.Id == id);
        }
        public virtual async Task Add(TEntity entity)
        {
            await this._dbSet.AddAsync(entity);
            await this.SaveChanges();
        }
        public virtual async Task InsertRange(IEnumerable<TEntity> entities)
        {
            await this._dbSet.AddRangeAsync(entities);
            await this.SaveChanges();
        }
        public virtual async Task Update(TEntity entity)
        {
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Modified;               
                await this.SaveChanges();
            }
        }
        public virtual async Task Remove(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            await this.SaveChanges();
        }
        public Task<bool> IsOwner(Guid id, string ownerId)
        {
            return this._dbSet.AnyAsync(i => i.Id == id && i.OwnerId == ownerId);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._context.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }       
        #endregion
    }
}
