using Microsoft.EntityFrameworkCore;
using WebAPITEST.Models;

namespace WebAPITEST.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }

    }
}
