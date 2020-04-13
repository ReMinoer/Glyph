using System;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Particles
{
    public sealed class ParticleEmitter : GlyphContainer, IUpdate, IDraw, ITimeUnscalable
    {
        private readonly SceneNode _sceneNode;
        private readonly List<IParticle> _particles;
        private readonly Period _period;
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
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

        public ParticleEmitter(SceneNode parentNode)
        {
            Enabled = true;

            Components.Add(_sceneNode = new SceneNode(parentNode));
            _period = new Period();
            _particles = new List<IParticle>();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            float[] spawnTimes;
            _period.Update(elapsedTime.GameTime, out spawnTimes);

            var particlesToRemove = new List<IParticle>();
            foreach (IParticle particle in _particles)
            {
                particle.Update(elapsedTime);

                if (!particle.Ended)
                    continue;

                particlesToRemove.Add(particle);
                particle.Dispose();
            }

            foreach (IParticle particleToRemove in particlesToRemove)
            {
                Components.Remove(particleToRemove);
                _particles.Remove(particleToRemove);
            }

            foreach (float spawnTime in spawnTimes)
            {
                IParticle particle = Factory();
                particle.SceneNode.SetParent(_sceneNode);

                Components.Add(particle);
                _particles.Add(particle);

                float startTime = spawnTime * elapsedTime.Scale - elapsedTime.GetDelta(this);
                particle.TimeOffset = startTime;
                particle.Update(elapsedTime);
            }
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer, drawer.Client, _sceneNode))
                return;

            foreach (IParticle particle in _particles)
                particle.Draw(drawer);
        }
    }
}