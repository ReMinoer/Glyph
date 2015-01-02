using Microsoft.Xna.Framework;

namespace Glyph.Transition
{
    public interface ITransition<T>
    {
        ITimingFunction Function { get; set; }
        T Start { get; set; }
        T End { get; set; }
        float Duration { get; set; }
        float MeanSpeed { set; }
        float Delay { get; set; }

        T Value { get; }
        float ElapsedTime { get; }

        bool IsEnd { get; }
        bool IsWaiting { get; }

        void Init(T start, T end, float duration, bool reset = false, bool fromEnd = false);
        void InitBySpeed(T start, T end, float meanSpeed, bool reset = false, bool fromEnd = false);

        void Reset(bool fromEnd = false);

        T Update(GameTime gameTime);
        T Update(GameTime gameTime, bool reverse);

        void ToggleReverse();

        T ProvisionalValue(float milliseconds);
        T ProvisionalValueRelative(float milliseconds);
    }
}