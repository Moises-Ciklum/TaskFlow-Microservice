using Microsoft.EntityFrameworkCore;
using CoupleCalendar.Core.Entities;

namespace CoupleCalendar.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CalendarEvent> Events { get; set; }
    }
}
