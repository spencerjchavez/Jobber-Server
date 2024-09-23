
using Jobber_Server.Models;
using Jobber_Server.Models.Contractors;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.DBContext
{
    public class JobberDbContext : DbContext
    {
        public JobberDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Contractor> Contractors { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<ContractorJobCategory> ContractorJobCategories { get; set; }
    }
}