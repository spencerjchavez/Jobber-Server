using System.Text.Json.Serialization;
using Jobber_Server.Models.Contractors;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models {
    
    [PrimaryKey(nameof(ContractorId), nameof(JobCategoryId))]
    public class ContractorJobCategory 
    {
        public int ContractorId { get; set; }

        public int JobCategoryId { get; set; }

        [JsonIgnore]
        public virtual Contractor Contractor { get; set; } = null!;
        public virtual JobCategory JobCategory { get; set; } = null!;
    }
}