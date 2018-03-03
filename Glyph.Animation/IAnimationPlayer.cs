using System;
using Glyph.Composition;

namespace Glyph.Animation
{
    public interface IAnimationPlayer : IUpdate, ITimeOffsetable, ITimeUnscalable
    {
        new bool UseUnscaledTime { get; set; }
        bool HasEnded { get; }
        bool IsPaused { get; }
        bool IsLooping { get; set; }
        void Play();
        void Pause();
        void Stop();
        event Action Ended;
    }
}