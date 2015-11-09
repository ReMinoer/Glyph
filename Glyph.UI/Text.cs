using Glyph.Animation;
using Glyph.Composition;
using Glyph.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI
{
    public class Text : GlyphContainer, ILoadContent, IDraw
    {
        public bool Visible { get; set; }
        public string Content { get; set; }
        public SpriteFont Font { get; set; }
        public string Asset { get; set; }
        public Vector2 Shadow { get; set; }
        public Color ShadowColor { get; set; }
        public SceneNode SceneNode { get; private set; }
        public SpriteTransformer SpriteTransformer { get; private set; }

        public Text()
            : base(2)
        {
            Components[0] = SceneNode = new SceneNode();
            Components[1] = SpriteTransformer = new SpriteTransformer();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            if (Asset != null)
                Font = contentLibrary.GetFont(Asset);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Font == null)
                return;

            if (Shadow != Vector2.Zero && ShadowColor.A != 0)
                spriteBatch.DrawString(Font, Content, SceneNode.Position + Shadow, ShadowColor,
                    SceneNode.Rotation, SpriteTransformer.Origin, SceneNode.Scale, SpriteTransformer.Effects, 0);

            spriteBatch.DrawString(Font, Content, SceneNode.Position, SpriteTransformer.Color,
                SceneNode.Rotation, SpriteTransformer.Origin, SceneNode.Scale, SpriteTransformer.Effects, 0);
        }

        public Vector2 MeasureString()
        {
            return Font.MeasureString(Content);
        }

        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }
    }
}