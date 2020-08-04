using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarEvents.Services
{
    public interface IInsertService<T>
    {
        Task<ResultHandler> Insert(T obj);
        Task<ResultHandler> InsertRange(IEnumerable<T> items);
    }
}
