using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public abstract class MeasurableTrajectoryBase : IMeasurableTrajectory
    {
        public abstract float Length { get; }

        public Vector2 Origin => GetPosition(0f);
        public Vector2 Destination => GetPosition(1f);

        Vector2 ITrajectory.GetPosition(float progress) => GetPosition(progress);
        protected abstract Vector2 GetPosition(float progress);

        public float GetDistance(float progress)
        {
            return Length * progress;
        }

        public Vector2 GetPosition(float progress, out float distance)
        {
            distance = GetDistance(progress);
            return GetPosition(progress);
        }

        public Vector2 GetPositionAtDistance(float distance)
        {
            return GetPositionAtDistance(distance, out _);
        }

        public Vector2 GetPositionAtDistance(float distance, out float progress)
        {
            distance = MathHelper.Clamp(distance, 0, Length);
            progress = Length <= 0 ? 1 : distance / Length;
            return GetPosition(progress);
        }
    }
}