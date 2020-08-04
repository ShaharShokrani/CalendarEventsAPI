using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{
    public interface IRemoveRepository<TEntity>
    {
        Task Remove(TEntity entity);
    }
}
