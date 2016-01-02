using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    internal class ProgressiveTrajectoryPlayer : TrajectoryPlayerBase<IProgressiveTrajectory>
    {
        public override float EstimatedDuration
        {
            get { return Time + (Trajectory.Length - Distance) / CurrentSpeed; }
        }

        public override float EstimatedLength
        {
            get { return Trajectory.Length; }
        }

        public override bool ReadOnlySpeed
        {
            get { return false; }
        }

        protected override void UpdateLocal(ElapsedTime elapsedTime)
        {
            if (State == TrajectoryPlayerState.Play)
            {
                float deltaTime = elapsedTime.GetDelta(this);
                Time += deltaTime;

                Distance += Speed * deltaTime;

                float newAdvance;
                Position = StartPosition + Trajectory.GetPosition(Distance, out newAdvance);
                Advance = newAdvance;

                Vector2 move = Position - LastPosition;
                Direction = move.Normalized();

                if (Advance >= 1f)
                {
                    Position = Trajectory.Destination;
                    Time -= (Distance - Trajectory.Length) / Speed;
                    Distance = Trajectory.Length;
                    Advance = 1;
                    End();
                }

                LastPosition = Position;
            }
            else
                Direction = Vector2.Zero;
        }
    }
}