using Microsoft.EntityFrameworkCore;
using AnimeMangaApi.Models;

namespace AnimeMangaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option) { }

        public DbSet<User> Users { get; set; }
        public DbSet<AnimeMangaEntry> AnimeMangaEntries { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}