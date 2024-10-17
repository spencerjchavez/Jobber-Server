using Microsoft.EntityFrameworkCore;

namespace Jobber_Server.Models.Contractors.Sector
{
    public class Sector
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Depth { get; set; }
        public int? ParentId { get; set; } = null;
        public int? NWId { get; set; } = null;
        public int? NEId { get; set; } = null;
        public int? SEId { get; set; } = null;
        public int? SWId { get; set; } = null;
        
        public Sector Parent { get; set; } = null!;        
        public Sector? NW { get; set; } = null;
        public Sector? NE { get; set; } = null;
        public Sector? SE { get; set; } = null;
        public Sector? SW { get; set; } = null;

        public virtual ICollection<ContractorSector> ContractorSectors { get; set; } = null!;

        public double Width() => 180.0 / Math.Pow(2, Depth);
        
        public bool HasChildren() => NEId != null;
        public double LatitudeNorth() => Latitude + Width() / 2; 
        public double LatitudeSouth() => Latitude - Width() / 2; 
        public double LongitudeWest() => Longitude - Width() / 2; 
        public double LongitudeEast() =>  Longitude + Width() / 2; 

    }
}