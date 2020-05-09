using DailyHeil.Functions.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyHeil.Functions.Db
{
    public class DailyHeilContext : DbContext
    {
        public DailyHeilContext(DbContextOptions<DailyHeilContext> options) : base(options)
        {
            
        }

        public DbSet<Newspaper> Newspapers { get; set; }

        public DbSet<Feed> Feeds { get; set; }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DailyHeilContext).Assembly);
        }
    }
}