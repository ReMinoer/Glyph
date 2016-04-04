using Diese.Injection;
using Glyph.Animation;
using Glyph.Particles;

namespace Glyph.Graphics.Particles
{
    public abstract class ParticleBuilder
    {
        private readonly IDependencyInjector _injector;
        protected PropertiesAnimationBuilder<StandardParticle> Animation { get; private set; }
        protected float? LifeTime { get; set; }

        protected ParticleBuilder(IDependencyInjector injector)
        {
            _injector = injector;
            Animation = new PropertiesAnimationBuilder<StandardParticle>();
        }

        public abstract void Configure();

        public IParticle Generate()
        {
            var particle = _injector.Resolve<StandardParticle>();
            Configure();

            IAnimation<StandardParticle> animation = Animation.Create();

            particle.LifeTime = LifeTime ?? animation.Duration;
            particle.AnimationPlayer.Animation = animation;

            return particle;
        }
    }
}