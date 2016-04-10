using System;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI.Simple
{
    public class SimpleBorder : GlyphComponent, ILoadContent, IBorder
    {
        private readonly RectangleSprite _rectangleSprite;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; }

        public OriginRectangle Bounds
        {
            get { return new OriginRectangle(SceneNode.Position, Size); }
        }

        public SimpleBorder(Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            Enabled = true;
            Visible = true;

            SceneNode = new SceneNode();
            Motion = new Motion(SceneNode);
            _rectangleSprite = new RectangleSprite(lazyGraphicsDevice);

            Color = Color.Black;
            Thickness = 1;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _rectangleSprite.LoadContent(contentLibrary);
        }

        public void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            drawer.SpriteBatchStack.Current.Draw(_rectangleSprite.Texture, Bounds.ToStruct(), Color);
        }
    }
}