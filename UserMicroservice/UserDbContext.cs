using Listable.UserMicroservice.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listable.UserMicroservice
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.DisplayName)
                .IsUnique();
        }
    }
}
