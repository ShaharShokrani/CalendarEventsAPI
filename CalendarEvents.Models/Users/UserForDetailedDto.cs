using System;

namespace CalendarEvents.Models
{
    public class UserForDetailedDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}
