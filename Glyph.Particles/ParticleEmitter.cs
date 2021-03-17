using System;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Particles
{
    public sealed class ParticleEmitter : GlyphObject, ITimeUnscalable
    {
        private readonly SceneNode _sceneNode;
        private readonly List<IParticle> _particles;
        private readonly Period _period;
        public Func<IParticle> Factory { get; set; }
        public bool UseUnscaledTime { get; set; }

        public float Interval
        {
            get { return _period.Interval; }
            set { _period.Interval = value; }
        }

        public Vector2 Center
        {
            get { return _sceneNode.LocalPosition; }
            set { _sceneNode.LocalPosition = value; }
        }

        public ParticleEmitter(GlyphResolveContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();

            _period = new Period();
            _particles = new List<IParticle>();

            Schedulers.Update.Plan(Update);
        }

        private void Update(ElapsedTime elapsedTime)
        {
            float[] spawnTimes;
            _period.Update(elapsedTime.GameTime, out spawnTimes);

            var particlesToRemove = new List<IParticle>();
            foreach (IParticle particle in _particles)
            {
                //particle.Update(elapsedTime);

                if (!particle.Ended)
                    continue;

                particlesToRemove.Add(particle);
                particle.Dispose();
            }

            foreach (IParticle particleToRemove in particlesToRemove)
            {
                Remove(particleToRemove);
                _particles.Remove(particleToRemove);
            }

            foreach (float spawnTime in spawnTimes)
            {
                IParticle particle = Factory();
                particle.SceneNode.SetParent(_sceneNode);

                Add(particle);
                _particles.Add(particle);

                float startTime = spawnTime * elapsedTime.Scale;
                //float startTime = spawnTime * elapsedTime.Scale - elapsedTime.GetDelta(this);

                particle.TimeOffset = startTime;
                //particle.Update(elapsedTime);
            }
        }
    }
}