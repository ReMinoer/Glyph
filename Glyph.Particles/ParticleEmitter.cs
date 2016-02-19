using System;
using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Particles
{
    public class ParticleEmitter : GlyphComposite<IParticle>, IEnableable, IUpdate, IDraw, ITimeUnscalable
    {
        protected readonly SceneNode SceneNode;
        private readonly Period _period;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Func<IParticle> Factory { get; set; }
        public bool UseUnscaledTime { get; set; }

        public float Interval
        {
            get { return _period.Interval; }
            set { _period.Interval = value; }
        }

        public Vector2 Center
        {
            get { return SceneNode.LocalPosition; }
            set { SceneNode.LocalPosition = value; }
        }

        public ParticleEmitter(SceneNode parentNode)
        {
            SceneNode = new SceneNode(parentNode);
            _period = new Period();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            float[] spawnTimes;
            _period.Update(elapsedTime.GameTime, out spawnTimes);

            var particlesToRemove = new List<IParticle>();
            foreach (IParticle particle in this)
            {
                particle.Update(elapsedTime);

                if (!particle.Ended)
                    continue;

                particlesToRemove.Add(particle);
                particle.Dispose();
            }

            foreach (IParticle particleToRemove in particlesToRemove)
                Remove(particleToRemove);

            foreach (float spawnTime in spawnTimes)
            {
                IParticle particle = Factory();
                particle.SceneNode.SetParent(SceneNode);
                Add(particle);

                float startTime = spawnTime * elapsedTime.Scale - elapsedTime.GetDelta(this);
                particle.SetTimeOffset(startTime);
                particle.Update(elapsedTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (IParticle particle in this)
                particle.Draw(spriteBatch);
        }
    }
}