namespace Glyph.Animation.Trajectories
{
    static public class TrajectoryExtensions
    {
        static public ITimedTrajectory Timed(this ITrajectory trajectory, float duration)
        {
            return new TimedTrajectory(trajectory, duration);
        }
    }
}