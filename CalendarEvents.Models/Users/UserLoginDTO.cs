using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarEvents.Models
{
    public class UserLoginDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
