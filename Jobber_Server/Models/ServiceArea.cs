namespace Jobber_Server.Models {
    public record ServiceArea 
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }

        public bool Contains(double latitude, double longitude)
        {
            return Math.Sqrt(Math.Pow(latitude - Latitude, 2) + Math.Pow(longitude - Longitude, 2)) * 6378 <= Radius;
        }
    }
}