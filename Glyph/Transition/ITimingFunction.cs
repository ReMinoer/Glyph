namespace Glyph.Transition
{
    public interface ITimingFunction
    {
        float GetValue(float t);
    }
}