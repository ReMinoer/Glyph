using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.StatusDisplayChannels
{
    public abstract class StatusDisplayChannel
    {
        public Dictionary<string, StatusDisplayText> Text { get; set; }
        public Vector2 ScreenMargin { get; set; }
        public bool OriginRight { get; set; }
        public bool OriginBottom { get; set; }
        public float Spacing { get; set; }

        public Vector2 Origin
        {
            get
            {
                return new Vector2(OriginRight ? Resolution.Size.X - ScreenMargin.X : ScreenMargin.X,
                    OriginBottom ? Resolution.Size.Y - ScreenMargin.Y : ScreenMargin.Y);
            }
        }

        protected StatusDisplayChannel()
        {
            Text = new Dictionary<string, StatusDisplayText>();

            ScreenMargin = new Vector2(5, 5);
            OriginRight = false;
            OriginBottom = false;
            Spacing = 0;
        }

        protected abstract void UpdateValues(GameTime gameTime);

        public void LoadContent(ContentLibrary ressources)
        {
            foreach (StatusDisplayText t in Text.Values)
                t.LoadContent(ressources);
        }

        public void Update(GameTime gameTime)
        {
            UpdateValues(gameTime);

            int i = 0;
            foreach (StatusDisplayText t in Text.Values)
            {
                Vector2 size = t.MeasureString();
                float x = OriginRight ? Origin.X - size.X : Origin.X;
                float y = (OriginBottom ? Origin.Y - size.Y - i * (size.Y + Spacing) : Origin.Y + i * (size.Y + Spacing));
                t.Position = new Vector2(x, y);
                i++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (StatusDisplayText t in Text.Values)
                t.Draw(spriteBatch);
        }
    }
}