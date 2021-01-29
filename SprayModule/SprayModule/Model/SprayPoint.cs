namespace SprayModule.Model
{
    /// <summary>
    /// This class holds information about a point which needs to be sprayed, namely a tree
    /// The class contains that latitude and longitude of the tree and a ratio of 0-1 that
    /// indicates that amount off herbicide that must be applied.
    /// sprayRatio of 1 indicates 600l/ha is required.
    /// </summary>
    public class SprayPoint
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public int SprayRatio { get; }

        public SprayPoint(double latitude, double longitude, int sprayRatio)
        {
            Latitude = latitude;
            Longitude = longitude;
            SprayRatio = sprayRatio;
        }

        public override string ToString()
        {
            return $"SprayPoint(Latitude={Latitude}, Longitude={Longitude}, SprayRatio={SprayRatio})";
        }
    }
}
