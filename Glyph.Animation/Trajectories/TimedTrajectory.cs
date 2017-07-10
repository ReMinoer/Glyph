using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public class TimedTrajectory : ITimedTrajectory
    {
        public ITrajectory Trajectory { get; }
        public float Duration { get; }
        public Vector2 Origin => Trajectory.Origin;
        public Vector2 Destination => Trajectory.Destination;

        public TimedTrajectory(ITrajectory trajectory, float duration)
        {
            Trajectory = trajectory;
            Duration = duration;
        }

        public Vector2 GetPositionAtTime(float time)
        {
            return GetPositionAtTime(time, out _);
        }

        public Vector2 GetPositionAtTime(float time, out float progress)
        {
            time = MathHelper.Clamp(time, 0, Duration);
            progress = Duration <= 0 ? 1 : time / Duration;
            return Trajectory.GetPosition(progress);
        }

        Vector2 ITrajectory.GetPosition(float progress)
        {
            return Trajectory.GetPosition(progress);
        }
    }
}