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
        public bool Enabled { get; set; }
        public TrajectoryPlayerState State { get; private set; }
        public TTrajectory Trajectory { get; set; }
        public float Time { get; protected set; }
        public float Distance { get; protected set; }
        public float Advance { get; protected set; }
        public bool UseUnscaledTime { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 Direction { get; protected set; }

        public float Speed
        {
            get { return State == TrajectoryPlayerState.Play ? CurrentSpeed : 0; }
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

        ITrajectory ITrajectoryPlayer.Trajectory
        {
            get { return Trajectory; }
        }

        protected TrajectoryPlayerBase()
        {
            State = TrajectoryPlayerState.End;
        }

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

            Advance = 0;
            Time = 0;
            Distance = 0;
            Direction = Vector2.Zero;
            CurrentSpeed = 0;

            State = TrajectoryPlayerState.Play;
        }

        public void Resume()
        {
            if (State == TrajectoryPlayerState.Pause)
                State = TrajectoryPlayerState.Play;
        }

        public void Pause()
        {
            if (State == TrajectoryPlayerState.Play)
                State = TrajectoryPlayerState.Pause;
        }

        protected abstract void UpdateLocal(ElapsedTime elapsedTime);

        protected void End()
        {
            State = TrajectoryPlayerState.End;
        }
    }
}