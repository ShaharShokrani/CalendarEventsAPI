using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarEvents.Models
{    
    public interface IGenericEntity
    {
        Guid Id { get; set; }
        string OwnerId { get; set; }
        DateTime CreateDate { get; set; }
        DateTime UpdateDate { get; set; }
    }
}
