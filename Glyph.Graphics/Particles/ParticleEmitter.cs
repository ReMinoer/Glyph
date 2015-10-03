using System;
using System.Collections.Generic;
using Glyph.Composition;

namespace Glyph.Graphics.Particles
{
    public class ParticleEmitter : GlyphComponent, IUpdate, IDraw, ITimeUnscalable
    {
        public bool Visible { get; private set; }
        public Func<IParticle> Factory { get; private set; }
        public bool UseUnscaledTime { get; set; }

        public float Interval
        {
            get { return _period.Interval; }
            set { _period.Interval = value; }
        }

        private readonly Period _period;
        private readonly List<IParticle> _particlesInstances;

        public ParticleEmitter()
        {
            _particlesInstances = new List<IParticle>();
            _period = new Period();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            float[] spawnTimes;
            _period.Update(elapsedTime.GameTime, out spawnTimes);

            var particlesToRemove = new List<IParticle>();
            foreach (IParticle particle in _particlesInstances)
            {
                particle.Update(elapsedTime);

                if (!particle.Ended)
                    continue;

                particlesToRemove.Add(particle);
                particle.Dispose();
            }

            foreach (IParticle particleToRemove in particlesToRemove)
                _particlesInstances.Remove(particleToRemove);

            foreach (float spawnTime in spawnTimes)
            {
                IParticle particle = Factory();
                particle.Initialize();

                float startTime = spawnTime * elapsedTime.Scale - elapsedTime.GetDelta(this);
                particle.SetTimeOffset(startTime);
                particle.Update(elapsedTime);

                _particlesInstances.Add(particle);
            }
        }

        public void Draw()
        {
            foreach (IParticle particle in _particlesInstances)
                particle.Draw();
        }
    }
}