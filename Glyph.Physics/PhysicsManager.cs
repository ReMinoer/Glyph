namespace Glyph.Physics
{
    public class PhysicsManager
    {
        public const float EarthGravity = 9.80665f;
        public float Gravity { get; set; }
        public float LengthUnit { get; set; }

        public PhysicsManager()
        {
            Gravity = EarthGravity;
            LengthUnit = 1f;
        }
    }
}