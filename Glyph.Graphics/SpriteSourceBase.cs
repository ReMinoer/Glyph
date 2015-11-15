using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public abstract class SpriteSourceBase : GlyphComponent, ISpriteSource
    {
        private Texture2D _texture;
        private Rectangle _rectangle;
        private Rectangle _defaultRectangle;

        public Texture2D Texture
        {
            get { return _texture; }
            protected set
            {
                _texture = value;

                Rectangle = null;
                _defaultRectangle = _texture != null
                    ? new Rectangle(0, 0, _texture.Width, _texture.Height)
                    : new Rectangle(0, 0, 0, 0);
            }
        }

        public Rectangle? Rectangle
        {
            get { return _rectangle; }
            set
            {
                if (value != null && Texture != null)
                    if (value.Value.Top < 0 && value.Value.Bottom > Texture.Height
                        || value.Value.Left < 0 && value.Value.Right > Texture.Width)
                        throw new ArgumentOutOfRangeException();

                _rectangle = value.GetValueOrDefault();
            }
        }

        protected SpriteSourceBase()
        {
            _defaultRectangle = new Rectangle(0, 0, 0, 0);
        }

        Rectangle ISpriteSource.GetDrawnRectangle()
        {
            return Rectangle ?? _defaultRectangle;
        }
    }
}