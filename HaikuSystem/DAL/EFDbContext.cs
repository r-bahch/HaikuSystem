using System.Data.Entity;
using HaikuSystem.Models;

namespace HaikuSystem.DAL
{
    public class EFDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Haiku> Haikus { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}