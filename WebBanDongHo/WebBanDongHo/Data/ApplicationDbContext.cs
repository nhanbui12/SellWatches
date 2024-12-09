using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebBanDongHo.Models;

namespace WebBanDongHo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
             builder.Entity<Products>()
            .Property(p => p.price)
            .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderDetails>()
           .Property(o => o.unitPrice)
           .HasColumnType("decimal(18, 2)");

            builder.Entity<OrderDetails>()
           .Property(o => o.total)
           .HasColumnType("decimal(18, 2)");
        }

        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<ApplicationUsers> ApplicationUsers { get; set; }

        public DbSet<ApplicationUserRoles> ApplicationUserRoles { get; set; }

        public DbSet<Comments> Comments { get; set; }
    }
}
