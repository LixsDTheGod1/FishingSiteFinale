using Microsoft.EntityFrameworkCore;
using MVC.Intro.Models;
using System.IO;

namespace MVC.Intro.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath;

        // За DI (използва се в Program.cs)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Пътят се задава в Program.cs
        }

        // За ръчно създаване (тестове, миграции)
        public AppDbContext()
        {
            var projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            var dataFolder = Path.Combine(projectRoot, "..", "..", "..", "Data"); // до корена
            var fullPath = Path.GetFullPath(dataFolder);
            Directory.CreateDirectory(fullPath);
            _dbPath = Path.Combine(fullPath, "products.db");
        }

        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Ако няма DI – използвай ръчния път
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}