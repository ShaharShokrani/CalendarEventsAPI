using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarEvents.Models
{
    public class EventPutRequest 
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string URL { get; set; }
        [IsNotEmpty(ErrorMessage = "Guid Id Is Empty")]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
