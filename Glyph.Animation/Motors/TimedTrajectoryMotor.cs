using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;

namespace Glyph.Animation.Motors
{
    public class TimedTrajectoryMotor : TrajectoryMotorBase<ITimedTrajectory>
    {
        protected override TrajectoryPlayerBase<ITimedTrajectory> TrajectoryPlayer { get; } = new TimedTrajectoryPlayer();

        public TimedTrajectoryMotor(Motion motion)
            : base(motion)
        {
        }
    }
}