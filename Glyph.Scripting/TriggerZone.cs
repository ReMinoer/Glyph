using System;
using Glyph.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Scripting
{
    public class TriggerZone : Trigger
    {
        private readonly Zone _zone;

        public TriggerZone(int x, int y, int w, int h, int layer)
        {
            _zone = new Zone(x, y, w, h, layer) {Color = Color.Blue};
        }

        public void LoadContent(ContentLibrary ressources)
        {
            _zone.LoadContent(ressources);
        }

        public void Update(GameObject gameObject)
        {
            Actif = gameObject.Hitbox.Intersects(_zone.Hitbox)
                    && (!(gameObject is ILayable)
                        || Math.Abs((gameObject as ILayable).Layer - _zone.Layer) < float.Epsilon);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _zone.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return base.ToString() + " " + _zone.Hitbox;
        }
    }
}