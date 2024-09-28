using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models.Contractors.Sector
{
    [PrimaryKey(nameof(LatitudinalSlice), nameof(LatitudinalSubSlice), nameof(LongitudinalSlice))]
    public record Sector
    {
        public short LatitudinalSlice { get; set; }
        public short LatitudinalSubSlice { get; set; }
        public short LongitudinalSlice { get; set; }

        public virtual ISet<ContractorSector>? ContractorSectors { get; set; }
    }
}