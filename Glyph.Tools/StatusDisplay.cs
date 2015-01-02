using System.Collections.Generic;
using Glyph.Input;
using Glyph.Input.StandardActions;
using Glyph.Tools.StatusDisplayChannels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools
{
    public class StatusDisplay : IHandleInput
    {
        public bool Active { get; set; }
        public bool Visible { get; set; }
        public List<StatusDisplayChannel> Channels { get; set; }

        public StatusDisplay()
        {
            Active = true;
            Visible = true;
            Channels = new List<StatusDisplayChannel>();
        }

        public void HandleInput(InputManager input)
        {
            if (input.IsActionDownNow(DeveloperActions.StatusDisplay))
                Visible = !Visible;
        }

        public void LoadContent(ContentLibrary ressources)
        {
            foreach (StatusDisplayChannel c in Channels)
                c.LoadContent(ressources);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            foreach (StatusDisplayChannel c in Channels)
                c.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (StatusDisplayChannel c in Channels)
                c.Draw(spriteBatch);
        }
    }
}