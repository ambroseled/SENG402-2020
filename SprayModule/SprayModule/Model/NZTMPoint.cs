namespace SprayModule.Model
{
    /// <summary>
    /// Model class to store a point in NZTM
    /// </summary>
    public class NZTMPoint
    {
        public double x { get; }
        public double y { get; }
        public int SprayRatio { get; }

        public NZTMPoint(double x, double y, int sprayRatio)
        {
            this.x = x;
            this.y = y;
            this.SprayRatio = sprayRatio;
        }
    }
}