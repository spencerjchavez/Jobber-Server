using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models.Contractors.Sector
{
    public class Sector
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Depth { get; set; }
        public int? Parent { get; set; } = null;
        public int? NW { get; set; } = null;
        public int? NE { get; set; } = null;
        public int? SE { get; set; } = null;
        public int? SW { get; set; } = null;
        
        /*public Sector Parent_Sector { get; set; } = null!;        
        public Sector? NW_Sector { get; set; } = null;
        public Sector? NE_Sector { get; set; } = null;
        public Sector? SE_Sector { get; set; } = null;
        public Sector? SW_Sector { get; set; } = null;*/

        public virtual ICollection<ContractorSector> ContractorSectors { get; set; } = null!;

        public double GetWidth() {
            return 180.0 / Math.Pow(2, Depth);
        }

        public bool HasChildren() { return NE != null; }
        public double LatitudeNorth() { return Latitude + GetWidth() / 2; }
        public double LatitudeSouth() { return Latitude - GetWidth() / 2; }
        public double LongitudeWest() { return Longitude - GetWidth() / 2; }
        public double LongitudeEast() { return Longitude + GetWidth() / 2; }

        public bool IntersectsServiceArea(ServiceArea serviceArea)
        {
                var latClosest = Math.Max(LatitudeSouth(), Math.Min(serviceArea.Latitude, LatitudeNorth()));
                var lonClosest = Math.Max(LongitudeWest(), Math.Min(serviceArea.Longitude, LongitudeEast()));
                var dLat = latClosest - serviceArea.Latitude;
                var dLon = lonClosest - serviceArea.Longitude;
                var sectorInServiceArea = Math.Sqrt(dLat * dLat + dLon * dLon) * 6378 <= serviceArea.Radius;
                return sectorInServiceArea;
        }
    }
}