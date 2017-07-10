using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;

namespace Glyph.Animation.Motors
{
    public class MeasurableTrajectoryMotor : TrajectoryMotorBase<IMeasurableTrajectory>
    {
        protected override TrajectoryPlayerBase<IMeasurableTrajectory> TrajectoryPlayer { get; } = new MeasurableTrajectoryPlayer();

        public MeasurableTrajectoryMotor(Motion motion)
            : base(motion)
        {
        }
    }
}