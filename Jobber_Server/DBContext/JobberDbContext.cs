
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
    }
}