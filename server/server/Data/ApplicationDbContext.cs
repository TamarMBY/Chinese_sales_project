using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Package> Packages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasMany(e => e.Gifts)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Gift>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                //entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
                entity.Property(e => e.IsDrawn);
                entity.HasOne(e => e.Category).WithMany(c => c.Gifts).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Donor).WithMany(d => d.Gifts).HasForeignKey(e => e.DonorId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Winner).WithMany(u => u.WonGifts).HasForeignKey(e => e.WinnerId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Donor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.LogoUrl).HasMaxLength(200);
                entity.HasMany(e => e.Gifts)
                .WithOne(g => g.Donor)
                .HasForeignKey(g => g.DonorId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(10);
                entity.HasMany(e => e.Purchases)
                .WithOne(p => p.Buyer)
                .HasForeignKey(p => p.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.WonGifts)
                .WithOne(g => g.Winner)
                .HasForeignKey(g => g.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount) .IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.IsDraft);
                entity.HasOne(e => e.Buyer).WithMany(b => b.Purchases).HasForeignKey(e => e.BuyerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Tickets).WithOne(t => t.Purchase).HasForeignKey(t => t.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Purchase>().HasMany(p => p.Packages).WithMany(pkg => pkg.Purchases)
                .UsingEntity(j => j.ToTable("PurchasePackages"));

            });
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.HasOne(e => e.Purchase).WithMany(p => p.Tickets).HasForeignKey(e => e.PurchaseId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Gift).WithMany(g => g.Tickets).HasForeignKey(e => e.GiftId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).IsRequired();
            });
        }
    }
}
