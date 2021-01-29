namespace SprayModule.Model
{
    /// <summary>
    /// This class holds a GPS location in the form of a latitude and a loongitude.
    /// </summary>
    public class GpsLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GpsLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
