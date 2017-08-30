using Glyph.Animation.Motors.Base;
using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;

namespace Glyph.Animation.Motors
{
    public class MeasurableTrajectoryMotor : TrajectoryMotorBase<IMeasurableTrajectory>
    {
        public float Speed
        {
            get => TrajectoryPlayer.Speed;
            set => TrajectoryPlayer.Speed = value;
        }

        protected override TrajectoryPlayerBase<IMeasurableTrajectory> TrajectoryPlayer { get; } = new MeasurableTrajectoryPlayer();

        public MeasurableTrajectoryMotor(Motion motion)
            : base(motion)
        {
        }
    }
}