﻿using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    internal class TimedTrajectoryPlayer : TrajectoryPlayerBase<ITimedTrajectory>
    {
        public override float EstimatedDuration => Trajectory.Duration;
        public override float EstimatedLength => Distance + (Trajectory.Duration - Time) * CurrentSpeed;
        public override bool ReadOnlySpeed => true;

        protected override void UpdateLocal(ElapsedTime elapsedTime)
        {
            if (!Playing)
            {
                Direction = Vector2.Zero;
                CurrentSpeed = 0;
                return;
            }
            
            float deltaTime = elapsedTime.GetDelta(this);
            Time += deltaTime;
            
            Position = StartPosition + Trajectory.GetPositionAtTime(Time, out float newProgress);
            Progress = newProgress;

            Vector2 move = Position - LastPosition;
            Direction = move.Normalized();

            Distance += move.Length();
            CurrentSpeed = Distance / deltaTime;

            if (Progress >= 1f)
            {
                Position = Trajectory.Destination;
                Distance -= (Time - Trajectory.Duration) * Speed;
                Time = Trajectory.Duration;
                Progress = 1;
                End();
            }

            LastPosition = Position;
        }
    }
}