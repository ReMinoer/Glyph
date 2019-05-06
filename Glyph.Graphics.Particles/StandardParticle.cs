using Niddle;
using Glyph.Animation;
using Glyph.Core;
using Glyph.Graphics.Renderer;
using Glyph.Particles;

namespace Glyph.Graphics.Particles
{
    public class StandardParticle : GlyphObject, IParticle
    {
        public float LifeTime { get; set; }
        public SceneNode SceneNode { get; private set; }
        public SpriteLoader SpriteLoader { get; private set; }
        public SpriteTransformer SpriteTransformer { get; private set; }
        public AnimationPlayer<StandardParticle> AnimationPlayer { get; private set; }

        public bool Ended
        {
            get { return AnimationPlayer.ElapsedTime >= LifeTime; }
        }

        public float TimeOffset
        {
            set { AnimationPlayer.TimeOffset = value; }
        }

        public StandardParticle(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            SpriteLoader = Add<SpriteLoader>();
            SpriteTransformer = Add<SpriteTransformer>();
            AnimationPlayer = Add<AnimationPlayer<StandardParticle>>();

            Add<SpriteRenderer>();
        }
    }
}