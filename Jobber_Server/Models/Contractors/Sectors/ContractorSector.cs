using System.Text.Json.Serialization;

namespace Jobber_Server.Models.Contractors.Sector
{
    public record ContractorSector
    {
        public int ContractorId { get; set; }
        public short LatitudinalSlice { get; set; }
        public short LatitudinalSubSlice { get; set; }
        public short LongitudinalSlice { get; set; }
                
        [JsonIgnore]
        public virtual Contractor Contractor { get; set; } = null!;
        [JsonIgnore]
        public virtual Sector Sector { get; set; } = null!;
    }
}