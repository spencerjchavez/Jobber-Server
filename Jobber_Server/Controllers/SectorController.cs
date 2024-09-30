using Jobber_Server.Models.Contractors.Sectors;

public namespace Jobber_Server.Controllers
{
    public class SectorController
    {
        private float[] latitudinalSlices = [0, 90];
        private float latitudinalSubSliceDistanceMultiplier = 1; //km
        private float longitudinalSliceDistanceMultiplier = 1; //km

        private short GetLatitudinalSliceIndex(float latitude)
        {
            latitude = Math.abs(latitude);
            if(latitude > 90) 
            {
                throw new Exception("Invalid latitude");
            }
            // binary search
            left = 0;
            right = latidutinalSlices.length;
            middle = left + (right - left) / 2;
            while(left != middle)
            {
                if(latitude < latitudinalSlices(middle))
                {
                    right = middle - 1;
                } else {
                    left = middle;
                }
            }
            return left;
        }

        private short GetLongitudinalSlice(float longitude, int latitudinalSliceIndex)
        {
            longitudinalSliceBaseAngle = Math.pow(2, latitudinalSliceIndex) * 3 / 6378;
            return (short) Math.abs(longitude) / longitudinalSliceBaseAngle * longitudinalSliceDistanceMultiplier;
        }

        private short GetLatitudinalSubSlice(float latitudinalOffsetFromSlice)
        {
            latitudinalOffsetFromSlice = Math.abs(latitudinalOffsetFromSlice);
            return (short) latitudinalOffsetFromSlice / 3.0 * latitudinalSubSliceDistanceMultiplier;
        }

        public static Sector GetSector(float latitude, float longitude, dbContext)
        {
            while(true)
            {
                var latSliceIndex = GetLatitudinalSliceIndex(latitude);
                var latSubSlice = GetLatitudinalSubSlice(latitude - latitudinalSlices[latSliceIndex]);
                var longSlice = GetLongitudinalSlice(longitude);
                var sector = Sector
                {
                    latitudinalSlice = latSliceIndex,
                    latitudinalSubSlice = latSubSlice,
                    longitudinalSlice = longSlice
                }
                var dbSector = dbContext.Sectors.FirstOrDefault(s => s == sector);
                if(dbSector == null)
                {
                    //work on this
                    latitudinalSliceDistance = latitudinalSlices[latSliceIndex + 1] - latitudinalSlices[latSliceIndex];
                    if(latitudinalSubSliceDistance < latitudinalSliceDistance)
                    {
                        latitudinalSubSliceDistanceMultiplier *= 2;
                    } else if (longitudinalSliceDistance > )
                    longitudinalSliceDistanceMultiplier *= 2;
                } else {
                    return dbSector;
                }
            } // Query DB for sector. If it doesn't exist, repeatedly double sector size until matching sector is found            }
        }
    }
}