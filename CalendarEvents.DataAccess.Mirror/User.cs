using System;
using System.Collections.Generic;

#nullable disable

namespace CalendarEvents.DataAccess.Mirror
{
    public partial class User
    {
        public int Id { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
    }
}
