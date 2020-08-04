using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{   
    public interface IGetRepository<TEntity>
    {
        //TODO: Implement IAsyncEnumarable
        Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);
        Task<TEntity> GetById(Guid id);
    }
}
