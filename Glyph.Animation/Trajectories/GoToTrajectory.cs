using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public sealed class GoToTrajectory : MeasurableTrajectoryBase
    {
        new public Vector2 Origin { get; set; }
        new public Vector2 Destination { get; set; }
        public EasingDelegate Easing { get; set; }
        public override float Length => (Destination - Origin).Length();

        public GoToTrajectory()
        {
        }

        public GoToTrajectory(Vector2 origin, Vector2 destination, EasingDelegate easing = null)
        {
            Origin = origin;
            Destination = destination;
            Easing = easing;
        }

        protected override Vector2 GetPosition(float progress)
        {
            return Origin + (Destination - Origin) * (Easing?.Invoke(progress) ?? progress);
        }
    }
}