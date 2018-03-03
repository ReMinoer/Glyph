using Glyph.Animation.Trajectories;
using Microsoft.Xna.Framework;

namespace Glyph.Animation
{
    public class TrajectoryAnimationBuilder : IAnimationBuilder<Vector2>
    {
        public ITimedTrajectory Trajectory { get; set; }
        public bool Loop { get; set; }

        public TrajectoryAnimationBuilder()
        {
        }

        public TrajectoryAnimationBuilder(ITimedTrajectory trajectory, bool loop = false)
        {
            Trajectory = trajectory;
            Loop = loop;
        }

        public IAnimation<Vector2> Create()
        {
            return new AnimationBuilder<Vector2>
            {
                Loop = Loop,
                [0, Trajectory.Duration] = (ref Vector2 animatable, float advance) => animatable = Trajectory.GetPosition(advance)
            }.Create();
        }
    }
}