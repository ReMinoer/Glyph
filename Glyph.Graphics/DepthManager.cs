using System;

namespace Glyph.Graphics
{
    static public class DepthManager
    {
        static public Predicate<float> VisibilityPredicate { get; set; } = x => true;
    }
}