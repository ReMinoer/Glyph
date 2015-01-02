namespace Glyph.Transition
{
    public class LinearFunction : ITimingFunction
    {
        public float GetValue(float t)
        {
            return t;
        }
    }
}