using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public abstract class SpriteSourceBase : GlyphComponent, ISpriteSource
    {
        private Rectangle? _rectangle;
        public abstract Texture2D Texture { get; }

        public Rectangle? Rectangle
        {
            get { return _rectangle; }
            set
            {
                if (value != null && Texture != null)
                    if (value.Value.Top < 0 && value.Value.Bottom > Texture.Height
                        || value.Value.Left < 0 && value.Value.Right > Texture.Width)
                        throw new ArgumentOutOfRangeException();

                _rectangle = value;
            }
        }

        public abstract event Action<ISpriteSource> Loaded;

        public Rectangle GetDrawnRectangle()
        {
            return (Rectangle ?? Texture?.Bounds) ?? Microsoft.Xna.Framework.Rectangle.Empty;
        }

        Vector2 ISpriteSource.GetDefaultOrigin()
        {
            return (Rectangle?.Size.ToVector2() / 2 ?? Texture?.Size() / 2) ?? Vector2.Zero;
        }
    }
}