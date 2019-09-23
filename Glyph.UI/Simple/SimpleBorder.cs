using System;
using System.Threading.Tasks;
using Diese.Collections;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Shapes;
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
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; }

        public TopLeftRectangle Bounds
        {
            get { return new TopLeftRectangle(SceneNode.Position, Size); }
        }

        public SimpleBorder(Func<GraphicsDevice> graphicsDeviceFunc)
        {
            Enabled = true;
            Visible = true;

            SceneNode = new SceneNode();
            Motion = new Motion(SceneNode);
            _rectangleSprite = new RectangleSprite(graphicsDeviceFunc);

            Color = Color.Black;
            Thickness = 1;
        }

        public async Task LoadContent(IContentLibrary contentLibrary)
        {
            await _rectangleSprite.LoadContent(contentLibrary);
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer, drawer.Client, SceneNode))
                return;

            drawer.SpriteBatchStack.Current.Draw(_rectangleSprite.Texture, Bounds.ToIntegers(), Color);
        }
    }
}