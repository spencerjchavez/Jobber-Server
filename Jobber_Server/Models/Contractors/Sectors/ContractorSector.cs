using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models.Contractors.Sector
{
    [PrimaryKey(nameof(SectorId), nameof(ContractorId))]
    public class ContractorSector
    {
        public int SectorId { get; set; }
        public int ContractorId { get; set; }
                
        [JsonIgnore]
        public virtual Contractor Contractor { get; set; } = null!;
        [JsonIgnore]
        public virtual Sector Sector { get; set; } = null!;
    }
}