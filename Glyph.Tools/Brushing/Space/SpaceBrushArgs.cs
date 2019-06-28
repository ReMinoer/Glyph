using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Space
{
    public class SpaceBrushArgs : ISpaceBrushArgs
    {
        public Vector2 Position { get; set; }
        public SpaceBrushArgs() {}
        public SpaceBrushArgs(Vector2 position) => Position = position;
    }
}