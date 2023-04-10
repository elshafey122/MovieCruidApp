using Microsoft.EntityFrameworkCore;

namespace CruidApplication.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public DbSet<Genre> Genres { set; get; }
        public DbSet<Film> Movies { set; get; }

    }
}
