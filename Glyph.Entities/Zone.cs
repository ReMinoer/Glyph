using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Entities
{
    public class Zone : ILayable
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Color Color { get; set; }
        public float Opacity { get; set; }

        [XmlIgnore]
        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)(Position.X - Camera.PositionByDefault.X * Layer), (int)Position.Y,
                    (int)Size.X, (int)Size.Y);
            }
        }
        private Texture2D _texture;

        public Zone()
            : this(0, 0, 0, 0, 0) {}

        public Zone(int x, int y, int w, int h, int layer)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(w, h);
            Layer = layer;
            Color = Color.Red;
            Opacity = 0.1f;
        }

        public int Layer { get; set; }

        public void LoadContent(ContentLibrary ressources)
        {
            _texture = ressources.GetTexture("square");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Hitbox, Color * Opacity);
        }
    }
}