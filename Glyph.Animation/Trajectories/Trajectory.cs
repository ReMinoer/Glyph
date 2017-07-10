using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories
{
    public delegate Vector2 TrajectoryDelegate(float advance);

    public class Trajectory : ITrajectory
    {
        public TrajectoryDelegate Delegate { get; }
        public Vector2 Origin => GetPosition(0f);
        public Vector2 Destination => GetPosition(1f);

        public Trajectory(TrajectoryDelegate trajectoryDelegate)
        {
            Delegate = trajectoryDelegate;
        }

        public Vector2 GetPosition(float advance)
        {
            return Delegate(advance);
        }
    }
}