using Jobber_Server.Models.Contractors.Sector;
using Jobber_Server.Utils;

namespace Jobber_Server.Models {
    public record ServiceArea 
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; } //km

        public bool Contains(double latitude, double longitude)
        {
            return EuclideanDistance.EuclideanDistanceKMSquared(Latitude, Longitude, latitude, longitude) <= Radius * Radius;
        }

        public bool Intersects(double latitude, double longitude, double radius) 
        {
            return EuclideanDistance.EuclideanDistanceKMSquared(Latitude, Longitude, latitude, longitude) <= (Radius + radius) * (Radius + radius);
        }

        /*
            Uses euclidan distances
            First checks that if the service area was bounded by a square, does that square intersect the sector?
            If yes, then we need to be sure that service area actually intersects the sector in its "square"'s corners.
            This boolean is represented as distance <= sectorCenterToCornerDistance + Radius or d <= scd + r
            Our distance is represented as distanceKMSquared because it is more efficient to not take the square root after doing the pythagorean theorem.
            Therefore, we square the terms above to get d^2 <= scd^2 + 2scd * r + r^2, which is what is returned below.
        */
        public bool Intersects(Sector sector)
        {
            var distanceDegrees = EuclideanDistance.EuclideanDistanceDegrees(Latitude, Longitude, sector.Latitude, sector.Longitude);
            var distanceLatitude = distanceDegrees[0];
            var distanceLongitude = distanceDegrees[1];
            if (distanceLatitude > sector.Width() / 2 + Radius) return false;
            if (distanceLongitude > sector.Width() / 2 + Radius) return false;

            var distanceLatitudeKM = distanceLatitude / 180.0 * Math.PI * 6378;
            var distanceLongitudeKM = distanceLongitude / 180.0 * Math.PI * 6378;
            var distanceKMSquared = distanceLatitudeKM * distanceLatitudeKM + distanceLongitudeKM * distanceLongitudeKM;

            var sectorWidthKM = sector.Width() / 180.0 * Math.PI * 6378;
            var sectorCenterToCornerDistanceKM = sectorWidthKM / Math.Sqrt(2); // from a^2 + b^2 = c^2 simplified
            return distanceKMSquared <= sectorCenterToCornerDistanceKM * sectorCenterToCornerDistanceKM + 2 * sectorCenterToCornerDistanceKM * Radius + Radius * Radius;
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