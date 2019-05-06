using Niddle;
using Glyph.Animation;
using Glyph.Particles;

namespace Glyph.Graphics.Particles
{
    public abstract class ParticleBuilder
    {
        private readonly IDependencyResolver _resolver;
        protected PropertiesAnimationBuilder<StandardParticle> Animation { get; private set; }
        protected float? LifeTime { get; set; }

        protected ParticleBuilder(IDependencyResolver resolver)
        {
            _resolver = resolver;
            Animation = new PropertiesAnimationBuilder<StandardParticle>();
        }

        public abstract void Configure();

        public IParticle Generate()
        {
            var particle = _resolver.Resolve<StandardParticle>();
            Configure();

            IAnimation<StandardParticle> animation = Animation.Create();

            particle.LifeTime = LifeTime ?? animation.Duration;
            particle.AnimationPlayer.Animation = animation;
            particle.AnimationPlayer.Play();

            return particle;
        }
    }
}