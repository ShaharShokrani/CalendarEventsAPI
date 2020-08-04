using CalendarEvents.Models;
using System;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{
    public interface IOwnerRepository<TEntity> where TEntity : class, IGenericEntity
    {
        Task<bool> IsOwner(Guid id, string ownerId);
    }
}
