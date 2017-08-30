using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors
{
    public class TrackMotor : MotorBase
    {
        public IMeasurableTrajectory Trajectory { get; set; }
        public float Speed { get; set; }
        public float Progress { get; set; }

        public TrackMotor(Motion motion)
            : base(motion)
        {
        }

        protected override Vector2 UpdateVelocity(ElapsedTime elapsedTime)
        {
            float distance = Trajectory.GetDistance(Progress);
            distance += Speed * elapsedTime.GetDelta(Motion);
            Vector2 newPosition = Trajectory.GetPositionAtDistance(distance, out float progress);
            Progress = progress;
            return newPosition - Motion.SceneNode.Position;
        }
    }
}