
using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;
using Jobber_Server.Models.Contractors.Sector;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.DBContext
{
    public class JobberDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<ContractorJobCategory> ContractorJobCategories { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<ContractorSector> ContractorSectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure self-referencing relationships
            modelBuilder.Entity<Sector>()
                .HasOne(s => s.Parent)
                .WithMany()
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<Sector>()
                .HasOne(s => s.NW)
                .WithMany()
                .HasForeignKey(s => s.NWId)
                .OnDelete(DeleteBehavior.ClientSetNull); 

            modelBuilder.Entity<Sector>()
                .HasOne(s => s.NE)
                .WithMany()
                .HasForeignKey(s => s.NEId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Sector>()
                .HasOne(s => s.SW)
                .WithMany()
                .HasForeignKey(s => s.SWId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Sector>()
                .HasOne(s => s.SE)
                .WithMany()
                .HasForeignKey(s => s.SEId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}