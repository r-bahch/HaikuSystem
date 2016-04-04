using System.Data.Entity;
using HaikuAPI.Models;

namespace HaikuAPI.DAL
{
    public class EFDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Haiku> Haikus { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}