using Diese.Injection;
using Glyph.Animation;

namespace Glyph.Graphics.Particles
{
    public abstract class ParticleBuilder
    {
        private readonly IDependencyInjector _injector;
        protected AnimationBuilder<StandardParticle> Animation { get; private set; }
        protected float? LifeTime { get; set; } 

        protected ParticleBuilder(IDependencyInjector injector)
        {
            _injector = injector;
            Animation = new AnimationBuilder<StandardParticle>();
        }

        public abstract void Configure();

        public IParticle Generate()
        {
            var particle = _injector.Resolve<StandardParticle>();
            Configure();

            StandardAnimation<StandardParticle> animation = Animation.Generate();

            particle.LifeTime = LifeTime ?? animation.Duration;
            particle.AnimationPlayer.Animation = animation;

            return particle;
        }
    }
}