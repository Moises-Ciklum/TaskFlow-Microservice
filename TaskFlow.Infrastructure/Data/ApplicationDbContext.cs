using Microsoft.EntityFrameworkCore;
using TaskFlow.Core.Entities;

namespace TaskFlow.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Esta línea le dice a EF Core: "Crea una tabla en Postgres llamada 'Tasks' basada en la clase TaskItem"
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
