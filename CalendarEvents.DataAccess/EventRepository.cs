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

    public class EventRepository : IGenericRepository<EventModel>
    {
        private readonly ApplicationDbContext _context = null;

        public EventRepository(ApplicationDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
        //TODO: Add paging support
        public virtual IAsyncEnumerable<EventModel> Get(IEnumerable<Expression<Func<EventModel, bool>>> filters = null, 
            Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>> orderBy = null, 
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<EventModel> query = this._context.Events;

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }                
            }

            return query.AsAsyncEnumerable();
        }

        public virtual async Task<EventModel> GetById(Guid id)
        {            
            return await this._context.Events.FirstOrDefaultAsync(item => item.Id == id);
        }
        public virtual async Task Add(EventModel entity)
        {
            await this._context.Events.AddAsync(entity);
            await this.SaveChanges();
        }
        public virtual async Task InsertRange(IEnumerable<EventModel> entities)
        {
            await this._context.Events.AddRangeAsync(entities);
            await this.SaveChanges();
        }
        public virtual async Task Update(EventModel entity)
        {
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Modified;               
                await this.SaveChanges();
            }
        }
        public virtual async Task Remove(EventModel entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                this._context.Events.Attach(entity);
            }
            this._context.Events.Remove(entity);
            await this.SaveChanges();
        }
        public Task<bool> IsOwner(Guid id, string ownerId)
        {
            return this._context.Events.AnyAsync(i => i.Id == id && i.OwnerId == ownerId);
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
