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

        public double Width() => 180.0 / Math.Pow(2, Depth);
        

        public bool HasChildren() => NE != null;
        public double LatitudeNorth() => Latitude + Width() / 2; 
        public double LatitudeSouth() => Latitude - Width() / 2; 
        public double LongitudeWest() => Longitude - Width() / 2; 
        public double LongitudeEast() =>  Longitude + Width() / 2; 
    }
}