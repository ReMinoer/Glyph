using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Entities
{
    public class Sprite
    {
        public virtual string Asset { get; set; }
        public virtual Vector2 Position { get; set; }
        public virtual Vector2 Direction { get; set; }
        public virtual float Speed { get; set; }
        public virtual Color Color { get; set; }
        public virtual float Opacity { get; set; }
        public virtual float Rotation { get; set; }
        public virtual float Scale { get; set; }
        public virtual Vector2 Origin { get; set; }
        public virtual SpriteEffects SpriteEffect { get; set; }

        [XmlIgnore]
        public virtual Texture2D Texture { get; set; }

        [XmlIgnore]
        public virtual Vector2 PositionScreen
        {
            get { return Position; }
        }

        [XmlIgnore]
        public virtual Rectangle RectangleSource
        {
            get
            {
                var rectangleSource = new Rectangle {X = 0, Y = 0, Width = Texture.Width, Height = Texture.Height};
                return rectangleSource;
            }
        }

        [XmlIgnore]
        public virtual Rectangle Hitbox
        {
            get
            {
                var hitbox = new Rectangle
                {
                    X = (int)Position.X,
                    Y = (int)Position.Y,
                    Width = (int)(RectangleSource.Width * Scale),
                    Height = (int)(RectangleSource.Height * Scale)
                };
                return hitbox;
            }
        }

        [XmlIgnore]
        public virtual Vector2 Center
        {
            get
            {
                Vector2 center = Vector2.Zero;
                center.X = Position.X + (RectangleSource.Width * Scale) / 2;
                center.Y = Position.Y + (RectangleSource.Height * Scale) / 2;
                return center;
            }
            set
            {
                Vector2 temp = Vector2.Zero;
                temp.X = value.X - (RectangleSource.Width * Scale) / 2;
                temp.Y = value.Y - (RectangleSource.Height * Scale) / 2;
                Position = temp;
            }
        }

        public virtual void Initialize()
        {
            Asset = "";
            Position = Vector2.Zero;
            Direction = Vector2.Zero;
            Speed = 0;
            Color = Color.White;
            Opacity = 1f;
            Origin = Vector2.Zero;
            Rotation = 0;
            Scale = 1;
        }

        public virtual void LoadContent(ContentLibrary ressources)
        {
            LoadContent(ressources, Asset);
        }

        public virtual void LoadContent(ContentLibrary ressources, string asset)
        {
            Asset = asset;
            if (Asset != "")
                Texture = ressources.GetTexture(Asset);
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
                spriteBatch.Draw(Texture, PositionScreen, null, Color * Opacity, Rotation, Origin, Scale, SpriteEffect,
                    0);
        }

        public override string ToString()
        {
            return base.ToString() + (Asset != "" ? " (" + Asset + ")" : "");
        }
    }
}