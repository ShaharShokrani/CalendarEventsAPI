using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarEvents.Models
{
    public class EventModelPostDTO
    {
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        [Required]
        public bool IsAllDay { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string ImagePath { get; set; }
    }
}
