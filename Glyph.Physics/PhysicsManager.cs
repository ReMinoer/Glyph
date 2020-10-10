namespace Glyph.Physics
{
    public class PhysicsManager
    {
        public const float EarthGravity = 9.80665f;
        public float Gravity { get; set; }
        public float PixelByMeter { get; set; } // pixel/m

        public PhysicsManager()
        {
            Gravity = EarthGravity;
            PixelByMeter = 1f;
        }
    }
}