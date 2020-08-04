using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{
    public interface IInsertRepository<TEntity>
    {
        Task Add(TEntity entity);
        Task InsertRange(IEnumerable<TEntity> entities);
    }
}
