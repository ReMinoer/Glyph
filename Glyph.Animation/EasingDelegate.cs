namespace Glyph.Animation
{
    public delegate float EasingDelegate(float advance);
    public delegate T ValueEasingDelegate<T>(T start, T end, float advance);
}