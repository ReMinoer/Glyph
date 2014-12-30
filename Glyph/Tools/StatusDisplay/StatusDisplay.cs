using System.Collections.Generic;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools
{
    public class StatusDisplay
    {
        public bool Active { get; set; }
        public bool Visible { get; set; }
        public List<StatusDisplayChannel> Channels { get; set; }
        private readonly Game _game;

        public StatusDisplay(Game game)
        {
            _game = game;
            Active = true;
            Visible = true;
            Channels = new List<StatusDisplayChannel>();
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
                c.Update(gameTime, _game);
        }

        public void HandleInput(InputManager input)
        {
            if (input.IsActionDownNow(DeveloperActions.StatusDisplay))
                Visible = !Visible;
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