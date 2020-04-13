using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    public abstract class TrajectoryPlayerBase<TTrajectory> : GlyphComponent, ITrajectoryPlayer<TTrajectory>
        where TTrajectory : ITrajectory
    {
        protected Vector2 LastPosition;
        protected float CurrentSpeed;
        public bool Playing => !Paused && !Ended;
        public bool Paused { get; private set; }
        public bool Ended { get; private set; }
        public TTrajectory Trajectory { get; set; }
        public float Time { get; protected set; }
        public float Distance { get; protected set; }
        public float Progress { get; protected set; }
        public bool UseUnscaledTime { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 Direction { get; protected set; }

        public float Speed
        {
            get => Playing ? CurrentSpeed : 0;
            set
            {
                if (ReadOnlySpeed)
                    throw new InvalidOperationException("Speed is read-only !");

                CurrentSpeed = value;
            }
        }

        public abstract bool ReadOnlySpeed { get; }
        public abstract float EstimatedDuration { get; }
        public abstract float EstimatedLength { get; }

        ITrajectory ITrajectoryPlayer.Trajectory => Trajectory;

        public virtual void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            UpdateLocal(elapsedTime);
        }

        public void Play(Vector2 startPosition)
        {
            StartPosition = startPosition;
            LastPosition = startPosition;

            Progress = 0;
            Time = 0;
            Distance = 0;
            Direction = Vector2.Zero;
            CurrentSpeed = 0;

            Paused = false;
            Ended = false;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        protected abstract void UpdateLocal(ElapsedTime elapsedTime);

        protected void End()
        {
            Ended = true;
        }
    }
}