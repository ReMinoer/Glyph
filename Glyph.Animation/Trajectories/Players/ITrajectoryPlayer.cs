﻿using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Animation.Trajectories.Players
{
    public interface ITrajectoryPlayer : IUpdate, ITimeUnscalable
    {
        bool Playing { get; }
        bool Paused { get; }
        bool Ended { get; }
        ITrajectory Trajectory { get; }
        float Progress { get; }
        float Time { get; }
        float Distance { get; }
        float EstimatedDuration { get; }
        float EstimatedLength { get; }
        Vector2 Position { get; }
        Vector2 StartPosition { get; }
        Vector2 Direction { get; }
        float Speed { get; set; }
        bool ReadOnlySpeed { get; }
        void Play(Vector2 startPosition);
        void Resume();
        void Pause();
    }

    public interface ITrajectoryPlayer<out T> : ITrajectoryPlayer
        where T : ITrajectory
    {
        new T Trajectory { get; }
    }
}