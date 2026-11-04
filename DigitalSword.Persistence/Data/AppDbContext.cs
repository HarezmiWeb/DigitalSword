using DigitalSword.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DigitalSword.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<SupportTeamMember> SupportTeamMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Order>(builder =>
            {
                builder.OwnsOne(o => o.TotalPrice);
            });
            modelBuilder.Entity<OrderItem>(builder =>
            {
                builder.OwnsOne(oi => oi.Price);
            });
            modelBuilder.Entity<Wallet>(builder =>
            {
                builder.OwnsOne(w => w.Balance);
            });
        }
    }
}
