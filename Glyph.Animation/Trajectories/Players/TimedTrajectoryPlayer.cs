using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    internal class TimedTrajectoryPlayer : TrajectoryPlayerBase<ITimedTrajectory>
    {
        public override float EstimatedDuration
        {
            get { return Trajectory.Duration; }
        }

        public override float EstimatedLength
        {
            get { return Distance + (Trajectory.Duration - Time) * CurrentSpeed; }
        }

        public override bool ReadOnlySpeed
        {
            get { return true; }
        }

        protected override void UpdateLocal(ElapsedTime elapsedTime)
        {
            if (State == TrajectoryPlayerState.Play)
            {
                float deltaTime = elapsedTime.GetDelta(this);
                Time += deltaTime;

                float newAdvance;
                Position = StartPosition + Trajectory.GetPosition(Time, out newAdvance);
                Advance = newAdvance;

                Vector2 move = Position - LastPosition;
                Direction = move.Normalized();

                Distance += move.Length();
                CurrentSpeed = Distance / deltaTime;

                if (Advance >= 1f)
                {
                    Position = Trajectory.Destination;
                    Distance -= (Time - Trajectory.Duration) * Speed;
                    Time = Trajectory.Duration;
                    Advance = 1;
                    End();
                }

                LastPosition = Position;
            }
            else
            {
                Direction = Vector2.Zero;
                CurrentSpeed = 0;
            }
        }
    }
}