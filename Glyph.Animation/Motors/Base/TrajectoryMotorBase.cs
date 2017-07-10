using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Motors.Base
{
    public abstract class TrajectoryMotorBase<TTrajectory> : MotorBase
        where TTrajectory : ITrajectory
    {
        public Referential Referential { get; set; }
        protected abstract TrajectoryPlayerBase<TTrajectory> TrajectoryPlayer { get; }

        public TTrajectory Trajectory
        {
            get => TrajectoryPlayer.Trajectory;
            set => TrajectoryPlayer.Trajectory = value;
        }

        protected TrajectoryMotorBase(Motion motion)
            : base(motion)
        {
        }

        protected override Vector2 UpdateVelocity(ElapsedTime elapsedTime)
        {
            TrajectoryPlayer.Update(elapsedTime);
            return TrajectoryPlayer.Position - Motion.SceneNode.GetPosition(Referential);
        }

        public void Play() => TrajectoryPlayer.Play(Vector2.Zero);
        public void Pause() => TrajectoryPlayer.Pause();
        public void Resume() => TrajectoryPlayer.Resume();
    }
}