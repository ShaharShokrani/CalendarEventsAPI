using System;
using System.Collections.Generic;

namespace CalendarEvents.Models
{
    public class GetRequest<TEntity>
    {        
        public IEnumerable<FilterStatement<TEntity>> Filters { get; set; }
        public OrderByStatement<TEntity> OrderBy { get; set; }
        public string IncludeProperties { get; set; }
    }
}
