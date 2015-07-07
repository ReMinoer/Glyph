using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;
using Microsoft.Xna.Framework;

// TODO : Splines

namespace Glyph.Animation
{
    [SinglePerParent]
    public class Motion : GlyphContainer, IEnableable, IUpdate, ITimeUnscalable
    {
        private readonly SceneNode _sceneNode;
        private Vector2 _direction;
        public bool Enabled { get; set; }
        public MoveType Type { get; private set; }
        public ITrajectoryPlayer TrajectoryPlayer { get; private set; }
        public bool UseUnscaledTime { get; set; }
        public Referential Referential { get; private set; }
        public float Speed { get; set; }

        public Vector2 Direction
        {
            get { return _direction; }
            set
            {
                Stop();
                _direction = value.Normalized();
            }
        }

        public Motion(SceneNode sceneNode)
            : base(1)
        {
            _sceneNode = sceneNode;

            Type = MoveType.Continuous;
            Referential = Referential.Local;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            switch (Type)
            {
                case MoveType.Continuous:

                    Vector2 moveDelta = Direction * Speed * elapsedTime.GetDelta(this);

                    switch (Referential)
                    {
                        case Referential.World:
                            _sceneNode.Position += moveDelta;
                            break;
                        case Referential.Local:
                            _sceneNode.LocalPosition += moveDelta;
                            break;
                    }

                    break;
                case MoveType.Trajectory:

                    if (!TrajectoryPlayer.ReadOnlySpeed)
                        TrajectoryPlayer.Speed = Speed;

                    TrajectoryPlayer.Update(elapsedTime);

                    switch (Referential)
                    {
                        case Referential.World:
                            _sceneNode.Position = TrajectoryPlayer.Position;
                            break;
                        case Referential.Local:
                            _sceneNode.LocalPosition = TrajectoryPlayer.Position;
                            break;
                    }

                    Direction = TrajectoryPlayer.Direction;
                    Speed = TrajectoryPlayer.Speed;

                    break;
            }
        }

        public void GoTo(float x, float y, Referential referential = Referential.Local)
        {
            GoTo(new Vector2(x, y), referential);
        }

        public void GoTo(Vector2 destination, Referential referential = Referential.Local)
        {
            FollowTrajectory(new GoToTrajectory(destination).AsProgressive(), referential);
        }

        public void GoTo(float x, float y, float duration, Referential referential = Referential.Local)
        {
            GoTo(new Vector2(x, y), duration, referential);
        }

        public void GoTo(Vector2 destination, float duration, Referential referential = Referential.Local)
        {
            GoTo(destination, duration, (advance => advance), referential);
        }

        public void GoTo(float x, float y, float duration, EasingDelegate easing,
            Referential referential = Referential.Local)
        {
            GoTo(new Vector2(x, y), duration, easing, referential);
        }

        public void GoTo(Vector2 destination, float duration, EasingDelegate easing,
            Referential referential = Referential.Local)
        {
            FollowTrajectory(new GoToTrajectory(destination).AsTimed(duration, easing), referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, Referential referential = Referential.Local)
        {
            PlayTrajectory(new ProgressiveTrajectoryPlayer {Trajectory = trajectory.AsProgressive()}, referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, float duration,
            Referential referential = Referential.Local)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory.AsTimed(duration)}, referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, float duration, EasingDelegate easing,
            Referential referential = Referential.Local)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory.AsTimed(duration, easing)}, referential);
        }

        public void FollowTrajectory(ITimedTrajectory trajectory, Referential referential = Referential.Local)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory}, referential);
        }

        public void FollowTrajectory(IProgressiveTrajectory trajectory, Referential referential = Referential.Local)
        {
            PlayTrajectory(new ProgressiveTrajectoryPlayer {Trajectory = trajectory}, referential);
        }

        public void Resume()
        {
            if (TrajectoryPlayer != null)
                TrajectoryPlayer.Resume();
        }

        public void Pause()
        {
            Direction = Vector2.Zero;

            if (TrajectoryPlayer != null)
                TrajectoryPlayer.Pause();
        }

        public void Stop()
        {
            Direction = Vector2.Zero;
            TrajectoryPlayer = null;
        }

        private void PlayTrajectory(ITrajectoryPlayer trajectoryPlayer, Referential referential)
        {
            Vector2 startPosition = referential == Referential.Local ? _sceneNode.LocalPosition : _sceneNode.Position;

            Components[0] = TrajectoryPlayer = trajectoryPlayer;
            TrajectoryPlayer.Play(startPosition);

            Referential = referential;

            Direction = Vector2.Zero;
            Type = MoveType.Trajectory;
        }

        public enum MoveType
        {
            Continuous,
            Trajectory
        }

        private class GoToTrajectory : StandardTrajectory
        {
            private readonly Vector2 _destination;

            public override Vector2 Destination
            {
                get { return _destination; }
            }

            public override float Length
            {
                get { return Destination.Length(); }
            }

            public GoToTrajectory(Vector2 destination)
            {
                _destination = destination;
            }

            public override Vector2 GetPosition(float advance)
            {
                return Destination * advance;
            }
        }
    }
}