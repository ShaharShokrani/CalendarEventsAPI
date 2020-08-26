using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarEvents.Models
{
    public class EventModel : IGenericEntity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string URL { get; set; }
        public string ImagePath {get;set;}
        public Guid Id { get; set; }
        public string Base64Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OwnerId { get; set; }
    }

    public class EventModelDTO
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string URL { get; set; }
        public string ImagePath { get; set; }
        public Guid Id { get; set; }
        public string Base64Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OwnerId { get; set; }
    }
}
