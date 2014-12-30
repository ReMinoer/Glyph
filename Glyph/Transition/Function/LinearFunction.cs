using Glyph.Transition;

namespace Glyph
{
    public class LinearFunction : ITimingFunction
    {
        public float GetValue(float t)
        {
            return t;
        }
    }
}