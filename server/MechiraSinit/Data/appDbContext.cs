using Microsoft.EntityFrameworkCore;
using MechiraSinit.Models;

namespace MechiraSinit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // הגדרת הטבלאות
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // כאן אנחנו אומרים לו במפורש: את המודל Donor תשמור בטבלה בשם "Donors"
            modelBuilder.Entity<Donor>().ToTable("Donors");
            modelBuilder.Entity<Gift>().ToTable("Gifts");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Purchase>().ToTable("Purchases");

            base.OnModelCreating(modelBuilder);
        }
    }
}