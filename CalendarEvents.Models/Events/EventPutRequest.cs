﻿using System;

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
        public string Base64Id { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string ImagePath { get; set; }
    }
}
