using System;
using System.Collections.Generic;

#nullable disable

namespace CalendarEvents.DataAccess.Mirror
{
    public partial class Event
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string Base64Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OwnerId { get; set; }
        public int Audience { get; set; }
    }
}