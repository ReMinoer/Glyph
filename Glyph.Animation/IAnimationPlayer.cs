using Glyph.Composition;

namespace Glyph.Animation
{
    public interface IAnimationPlayer : IUpdate, ITimeOffsetable, ITimeUnscalable
    {
        new bool UseUnscaledTime { get; set; }
        bool Ended { get; }
        bool Paused { get; }
        bool IsLooping { get; set; }
        void Play();
        void Pause();
        void Stop();
    }
}