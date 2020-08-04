using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarEvents.Services
{
    public interface IUpdateService<T>
    {
        Task<ResultHandler> Update(T obj);
    }
}
