using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    internal class MeasurableTrajectoryPlayer : TrajectoryPlayerBase<IMeasurableTrajectory>
    {
        public override float EstimatedDuration => Time + (Trajectory.Length - Distance) / CurrentSpeed;
        public override float EstimatedLength => Trajectory.Length;
        public override bool ReadOnlySpeed => false;

        protected override void UpdateLocal(ElapsedTime elapsedTime)
        {
            if (!Playing)
            {
                Direction = Vector2.Zero;
                return;
            }
            
            float deltaTime = elapsedTime.GetDelta(this);
            Time += deltaTime;
            Distance += Speed * deltaTime;

            Position = StartPosition + Trajectory.GetPositionAtDistance(Distance, out float newProgress);
            Progress = newProgress;

            Vector2 move = Position - LastPosition;
            Direction = move.Normalized();

            if (Progress >= 1f)
            {
                Position = Trajectory.Destination;
                Time -= (Distance - Trajectory.Length) / Speed;
                Distance = Trajectory.Length;
                Progress = 1;
                End();
            }

            LastPosition = Position;
        }
    }
}