using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarEvents.Models
{
    public class UserRegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "You must specify password between 6 and 20 characters")]
        public string Password { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserRegisterDTO()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}