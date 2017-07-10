using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public sealed class GoToTrajectory : MeasurableTrajectoryBase
    {
        public Vector2 Origin { get; }
        public Vector2 Destination { get; }
        public EasingDelegate Easing { get; }
        public override float Length => (Destination - Origin).Length();

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