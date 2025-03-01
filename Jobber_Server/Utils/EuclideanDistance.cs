namespace Jobber_Server.Utils 
{
    class EuclideanDistance
    {
        public static double EuclideanDistanceKMSquared(double lat1, double lon1, double lat2, double lon2)
        {
            var latDistanceKm = (lat1 - lat2) * Math.PI / 180 * 6378;
            var lonDistanceDegrees = Math.Abs(lon1 - lon2);
            if (lonDistanceDegrees > 180.0)
            {
                lonDistanceDegrees = 360.0 - lonDistanceDegrees;
            }
            var lonDistanceKm = lonDistanceDegrees * Math.PI / 180 * 6378 * Math.Cos((lat1 + lat2) / 2 * Math.PI / 180);
            return latDistanceKm * latDistanceKm + lonDistanceKm * lonDistanceKm;
        }

        public static IList<double> EuclideanDistanceDegrees(double lat1, double lon1, double lat2, double lon2)
        {
            var latDistance = Math.Abs(lat1 - lat2);
            var lonDistance = Math.Abs(lon1 - lon2);
            if (lonDistance > 180.0)
            {
                lonDistance = 360.0 - lonDistance;
            }
            return [latDistance, lonDistance];
        }

    }
}
