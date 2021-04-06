using CalendarEvents.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace CalendarEvents.DataAccess
{
    public class ApplicationDbContext : DbContext
    {        
        public ApplicationDbContext() {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //TODO: see if method override is needed.
        //protected override void OnConfiguring(DbContextOptionsBuilder builder)
        //{
        //    if (!builder.IsConfigured)
        //    {
        //        builder.UseSqlite("Data Source=CalendarEvent.db");
        //    }

        //    base.OnConfiguring(builder);
        //}

        public virtual DbSet<EventModel> Events { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
    }
}
