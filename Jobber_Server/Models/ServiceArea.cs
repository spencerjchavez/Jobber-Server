using Jobber_Server.Models.Contractors.Sector;

namespace Jobber_Server.Models {
    public record ServiceArea 
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; } //km

        public bool Contains(double latitude, double longitude)
        {
            var latDistanceKm = (Latitude - latitude) * Math.PI / 180 * 6378;
            var lonDistanceKm = (Longitude - longitude) * Math.PI / 180 * 6378 * Math.Cos(Latitude * Math.PI / 180);
            return Math.Sqrt(latDistanceKm * latDistanceKm + lonDistanceKm * lonDistanceKm) <= Radius;
        }

        public bool ServesEntireSector(Sector sector)
        {
            return Contains(sector.LatitudeNorth(), sector.LongitudeWest()) &&
                    Contains(sector.LatitudeNorth(), sector.LongitudeEast()) &&
                    Contains(sector.LatitudeSouth(), sector.LongitudeEast()) &&
                    Contains(sector.LatitudeSouth(), sector.LongitudeWest());   
        }

        public bool ServesSector(Sector sector)
        {
                var latClosest = Math.Max(sector.LatitudeSouth(), Math.Min(Latitude, sector.LatitudeNorth()));
                var lonClosest = Math.Max(sector.LongitudeWest(), Math.Min(Longitude, sector.LongitudeEast()));
                var latDistanceKm = (latClosest - Latitude) * Math.PI / 180 * 6378;
                var lonDistanceKm = (lonClosest - Longitude) * Math.PI / 180 * 6378 * Math.Cos(latClosest * Math.PI / 180);
                var sectorInServiceArea = Math.Sqrt(latDistanceKm * latDistanceKm + lonDistanceKm * lonDistanceKm) <= Radius;
                return sectorInServiceArea;
        }
    }
}