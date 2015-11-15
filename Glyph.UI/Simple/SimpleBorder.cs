using Glyph.Composition;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI.Simple
{
    public class SimpleBorder : GlyphComponent, ILoadContent, ISizableBorder
    {
        private readonly RectangleSprite _rectangleSprite;
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; }
        public OriginRectangle Bounds { get; private set; }

        public SimpleBorder(RectangleSprite rectangleSprite)
        {
            _rectangleSprite = rectangleSprite;

            Bounds = new OriginRectangle();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _rectangleSprite.LoadContent(contentLibrary);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            spriteBatch.Draw(_rectangleSprite.Texture, Bounds.ToStruct(), Color);
        }
    }
}