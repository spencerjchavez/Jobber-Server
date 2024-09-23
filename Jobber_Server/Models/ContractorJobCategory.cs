using Jobber_Server.Models.Contractors;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models {
    
    [PrimaryKey(nameof(ContractorId), nameof(JobCategoryId))]
    public record ContractorJobCategory 
    {
        public int ContractorId { get; set; }

        public int JobCategoryId { get; set; }

        public virtual Contractor Contractor { get; set; } = null!;
        public virtual JobCategory JobCategory { get; set; } = null!;
    }
}