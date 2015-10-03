using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;

namespace Glyph.Graphics.Particles
{
    public class StandardParticle : GlyphObject, IParticle
    {
        public float LifeTime { get; set; }
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public ISpriteSource SpriteSource { get; private set; }
        public SpriteTransformer Transformer { get; private set; }
        public StandardAnimationPlayer<StandardParticle> AnimationPlayer { get; private set; }

        public bool Ended
        {
            get { return AnimationPlayer.ElapsedTime >= LifeTime; }
        }

        public StandardParticle(IDependencyInjector injector)
            : base(injector)
        {
            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            SpriteSource = Add<SpriteLoader>();
            Transformer = Add<SpriteTransformer>();
            AnimationPlayer = Add<StandardAnimationPlayer<StandardParticle>>();

            Add<SpriteRenderer>();
        }

        public void SetTimeOffset(float offset)
        {
            AnimationPlayer.SetTimeOffset(offset);
        }
    }
}