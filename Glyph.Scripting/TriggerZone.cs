using System;
using Glyph.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Scripting
{
    public class TriggerZone : Trigger
    {
        public Zone Zone { get; private set; }

        public TriggerZone(string name, int x, int y, int w, int h, int layer, bool uniqueUse)
            : base(name, uniqueUse)
        {
            Zone = new Zone(x, y, w, h, layer) {Color = Color.Blue};
        }

        public void LoadContent(ContentLibrary ressources)
        {
            Zone.LoadContent(ressources);
        }

        public void Update(GameObject gameObject)
        {
            Enable = gameObject.Hitbox.Intersects(Zone.Hitbox)
                     && (!(gameObject is ILayable)
                         || Math.Abs((gameObject as ILayable).Layer - Zone.Layer) < float.Epsilon);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Zone.Draw(spriteBatch);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Name, Zone.Hitbox, Enable);
        }
    }
}