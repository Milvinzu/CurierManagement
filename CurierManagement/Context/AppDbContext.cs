using CurierManagement.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryPackage> DeliveryPackages { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }

        static AppDbContext()
        {
            Batteries.Init();
            

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "courierdb.sqlite");
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Courier>()
                .Property(c => c.Name)
                .IsRequired();

            modelBuilder.Entity<Courier>()
                .HasMany(c => c.Packages)
                .WithOne(p => p.Courier)
                .HasForeignKey(p => p.CourierId);

            modelBuilder.Entity<DeliveryPackage>()
                .HasMany(p => p.Route)
                .WithOne(r => r.DeliveryPackage)
                .HasForeignKey(r => r.DeliveryPackageId);

            modelBuilder.Entity<DeliveryPackage>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.DeliveryPackage)
                .HasForeignKey(o => o.DeliveryPackageId);
        }
    }
}
