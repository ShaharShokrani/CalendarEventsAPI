using CalendarEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarEvents.DataAccess
{
    public class ApplicationDbContext : DbContext
    {        
        public ApplicationDbContext() {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=CalendarEventsAPIDb;Integrated Security=True");
//            }
//        }

        public virtual DbSet<EventModel> Events { get; set; }
        public virtual DbSet<UserModel> Users { get; set; }
    }
}
