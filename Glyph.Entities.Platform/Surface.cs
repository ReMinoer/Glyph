using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Entities.Platform
{
    public class Surface
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Layer { get; set; }

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

        public Surface()
            : this(0, 0, 0, 0, 0) {}

        public Surface(int x, int y, int w, int h, float layer)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(w, h);
            Layer = layer;
            Color = Color.Red;
            Opacity = 0.1f;
        }

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