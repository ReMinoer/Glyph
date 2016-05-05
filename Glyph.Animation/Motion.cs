using System;
using Glyph.Animation.Trajectories;
using Glyph.Animation.Trajectories.Players;
using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Animation
{
    // TODO : Splines
    // TODO : Space unit length (pixel to meter)
    [SinglePerParent]
    public class Motion : GlyphContainer<ITrajectoryPlayer>, IEnableable, IUpdate, ITimeUnscalable
    {
        public enum MoveType
        {
            Dynamic,
            Steering,
            Trajectory
        }

        private readonly SceneNode _sceneNode;
        private Vector2 _direction = Vector2.Zero;
        private Vector2 _destination = Vector2.Zero;
        private float _angularSpeed = float.MaxValue;
        public bool Enabled { get; set; }
        public float Speed { get; set; }
        public bool UseUnscaledTime { get; set; }
        public bool AffectsRotation { get; set; }
        public Motion.MoveType Type { get; private set; }
        public Referential Referential { get; private set; }

        private ITrajectoryPlayer _trajectoryPlayer;

        public ITrajectoryPlayer TrajectoryPlayer
        {
            get { return _trajectoryPlayer; }
            set { Components.Replace(ref _trajectoryPlayer, value); }
        }

        public Vector2 Direction
        {
            get { return _direction; }
            set
            {
                if (Type != MoveType.Dynamic)
                    Stop();

                _direction = value.Normalized();
            }
        }

        public Vector2 Destination
        {
            get { return _destination; }
            set
            {
                if (Type != MoveType.Steering)
                {
                    Stop();
                    Type = MoveType.Steering;
                }
                _destination = value;
            }
        }

        public float AngularSpeed
        {
            get { return _angularSpeed; }
            set
            {
                if (Type != MoveType.Steering)
                {
                    Stop();
                    Type = MoveType.Steering;
                }
                _angularSpeed = value;
            }
        }

        public Motion(SceneNode sceneNode)
        {
            Enabled = true;

            _sceneNode = sceneNode;

            Type = MoveType.Dynamic;
            Referential = Referential.Local;
            Direction = Vector2.Zero;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            Vector2 moveDelta;
            switch (Type)
            {
                case MoveType.Dynamic:

                    moveDelta = Direction * Speed * elapsedTime.GetDelta(this);

                    switch (Referential)
                    {
                        case Referential.World:
                            _sceneNode.Position += moveDelta;
                            break;
                        case Referential.Local:
                            _sceneNode.LocalPosition += moveDelta;
                            break;
                    }

                    _angularSpeed = 0;

                    break;

                case MoveType.Steering:

                    Vector2 position;
                    switch (Referential)
                    {
                        case Referential.World:
                            position = _sceneNode.Position;
                            break;
                        case Referential.Local:
                            position = _sceneNode.LocalPosition;
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    Vector2 remainingDistance = Destination - position;
                    if (remainingDistance == Vector2.Zero)
                        Stop();

                    _direction = (Direction.Length().EqualsZero()
                                    ? remainingDistance
                                    : Direction.RotateToward(remainingDistance, AngularSpeed, elapsedTime.GetDelta(this))
                                    ).Normalized();

                    moveDelta = Direction * Speed * elapsedTime.GetDelta(this);

                    switch (Referential)
                    {
                        case Referential.World:
                            _sceneNode.Position += moveDelta;
                            break;
                        case Referential.Local:
                            _sceneNode.LocalPosition += moveDelta;
                            break;
                    }

                    remainingDistance = Destination - position;
                    if (Vector2.Dot(remainingDistance, moveDelta) <= 0)
                    {
                        switch (Referential)
                        {
                            case Referential.World:
                                _sceneNode.Position = Destination;
                                break;
                            case Referential.Local:
                                _sceneNode.LocalPosition = Destination;
                                break;
                            default:
                                throw new NotSupportedException();
                        }

                        Stop();
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

                    _destination = TrajectoryPlayer.Trajectory.Destination;
                    _angularSpeed = MathHelper.WrapAngle(TrajectoryPlayer.Direction.ToRotation() - Direction.ToRotation());
                    _direction = TrajectoryPlayer.Direction;
                    Speed = TrajectoryPlayer.Speed;

                    break;
            }

            if (AffectsRotation)
            {
                switch (Referential)
                {
                    case Referential.World:
                        _sceneNode.Rotation = Direction.ToRotation();
                        break;
                    case Referential.Local:
                        _sceneNode.LocalRotation = Direction.ToRotation();
                        break;
                }
            }
        }

        public void GoTo(float x, float y, Referential referential)
        {
            GoTo(new Vector2(x, y), referential);
        }

        public void GoTo(Vector2 destination, Referential referential)
        {
            FollowTrajectory(new GoToTrajectory(destination).AsProgressive(), referential);
        }

        public void GoTo(float x, float y, float duration, Referential referential)
        {
            GoTo(new Vector2(x, y), duration, referential);
        }

        public void GoTo(Vector2 destination, float duration, Referential referential)
        {
            GoTo(destination, duration, (advance => advance), referential);
        }

        public void GoTo(float x, float y, float duration, EasingDelegate easing, Referential referential)
        {
            GoTo(new Vector2(x, y), duration, easing, referential);
        }

        public void GoTo(Vector2 destination, float duration, EasingDelegate easing, Referential referential)
        {
            FollowTrajectory(new GoToTrajectory(destination).AsTimed(duration, easing), referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, Referential referential)
        {
            PlayTrajectory(new ProgressiveTrajectoryPlayer {Trajectory = trajectory.AsProgressive()}, referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, float duration, Referential referential)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory.AsTimed(duration)}, referential);
        }

        public void FollowTrajectory(IStandardTrajectory trajectory, float duration, EasingDelegate easing, Referential referential)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory.AsTimed(duration, easing)}, referential);
        }

        public void FollowTrajectory(ITimedTrajectory trajectory, Referential referential)
        {
            PlayTrajectory(new TimedTrajectoryPlayer {Trajectory = trajectory}, referential);
        }

        public void FollowTrajectory(IProgressiveTrajectory trajectory, Referential referential)
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
            Type = MoveType.Dynamic;

            _direction = Vector2.Zero;
            _destination = Vector2.Zero;
            _angularSpeed = float.MaxValue;
            TrajectoryPlayer = null;
        }

        private void PlayTrajectory(ITrajectoryPlayer trajectoryPlayer, Referential referential)
        {
            Vector2 startPosition = referential == Referential.Local ? _sceneNode.LocalPosition : _sceneNode.Position;

            TrajectoryPlayer = trajectoryPlayer;
            TrajectoryPlayer.Play(startPosition);

            Referential = referential;

            Direction = Vector2.Zero;
            Type = MoveType.Trajectory;
        }

        private sealed class GoToTrajectory : StandardTrajectory
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