using Glyph.Composition;

namespace Glyph.Animation
{
    public interface IAnimationPlayer : IUpdate, ITimeOffsetable
    {
        bool Ended { get; }
        bool Paused { get; }
        void Play();
        void Pause();
        void Stop();
    }
}