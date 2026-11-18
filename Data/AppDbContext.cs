using BookingManagement.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(customer => customer.Email)
                .IsUnique();
        }
    }
}
