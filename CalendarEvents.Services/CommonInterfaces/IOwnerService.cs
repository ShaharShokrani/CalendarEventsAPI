using System;
using System.Threading.Tasks;

namespace CalendarEvents.Services
{
    public interface IOwnerService<T>
    {        
        Task<ResultHandler<bool>> IsOwner(Guid id, string ownerId);
    }
}
