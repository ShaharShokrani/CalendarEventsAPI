using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarEvents.Services
{    
    public interface IGetService<TEntity>
    {
        Task<ResultHandler<IEnumerable<TEntity>>> Get(IEnumerable<FilterStatement<TEntity>> filterStatements,
                                                OrderByStatement<TEntity> orderBy = null,
                                                string includeProperties = "");
        Task<ResultHandler<TEntity>> GetById(Guid id);
    }
}
