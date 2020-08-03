using System.Threading.Tasks;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI
{
    public class Text : GlyphObject, IControl
    {
        public string Content { get; set; }
        public SpriteFont Font { get; set; }
        public string Asset { get; set; }
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Shadow? Shadow { get; set; }
        public SpriteTransformer SpriteTransformer { get; private set; }

        public Text(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            SpriteTransformer = Add<SpriteTransformer>();
            Motion = Add<Motion>();

            Schedulers.LoadContent.Plan(LoadContentLocal);
            Schedulers.Draw.Plan(DrawLocal);
        }

        public async Task LoadContentLocal(IContentLibrary contentLibrary)
        {
            if (Asset != null)
                Font = await contentLibrary.GetAsset<SpriteFont>(Asset).GetContentAsync();
        }

        public void DrawLocal(IDrawer drawer)
        {
            if (!Visible || Font == null)
                return;

            if (Shadow != null)
                drawer.SpriteBatchStack.Current.DrawString(Font, Content, SceneNode.Position + Shadow.Value.Position, Shadow.Value.Color,
                    SceneNode.Rotation, SpriteTransformer.Origin, SceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, 0);

            drawer.SpriteBatchStack.Current.DrawString(Font, Content, SceneNode.Position, SpriteTransformer.Color,
                SceneNode.Rotation, SpriteTransformer.Origin, SceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, 0);
        }

        public Vector2 MeasureContent()
        {
            return Font.MeasureString(Content);
        }
    }
}