using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Entities
{
    public class GameObject : AnimSprite
    {
        public virtual float Depth { get; set; }
        public virtual Orientation Orientation { get; set; }

        [XmlIgnore]
        public virtual Vector2 Destination { get; set; }

        [XmlIgnore]
        public override Vector2 PositionScreen
        {
            get { return Position.Substract(Camera.PositionByDefault.X * Depth, 0); }
        }
        protected readonly List<Color[]> PixelData;

        public GameObject()
        {
            PixelData = new List<Color[]>();
        }

        public override void Initialize()
        {
            base.Initialize();
            Depth = 0;
            Orientation = Orientation.Down;
        }

        public override void LoadContent(ContentLibrary ressources)
        {
            LoadContent(ressources, Asset);
        }

        public override void LoadContent(ContentLibrary ressources, string asset, int frameX = 1, int frameY = 1)
        {
            base.LoadContent(ressources, asset, frameX, frameY);

            PixelData.Clear();
            for (int i = 0; i < TextureSplit.Count; i++)
            {
                PixelData.Add(new Color[Texture.Width * Texture.Height]);
                Texture.GetData(PixelData[i]);
            }
        }

        protected override void UpdateAnimation(GameTime gameTime)
        {
            base.UpdateAnimation(gameTime);

            DirectionToOrientation();
            OrientationToMirror();
        }

        public virtual bool ToDestinationLinear(GameTime gameTime)
        {
            Vector2 diff = Destination - Center;

            if (diff != Vector2.Zero)
            {
                Direction = Vector2.Normalize(diff);
                Vector2 modif = Direction * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (!(modif.Length() >= diff.Length()))
                    return false;

                Center = Destination;
                Direction = Vector2.Zero;
                return true;
            }
            Center = Destination;
            Direction = Vector2.Zero;
            return true;

            //Vector2 ecart = Destination - Centre;
            //Vector2 lastDep = Direction * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //if (ecart != Vector2.Zero && ecart.Length() >= lastDep.Length())
            //{
            //    Direction = Vector2.Normalize(ecart);
            //    SetSensDirection();
            //    return false;
            //}
            //else
            //{
            //    Centre = Destination;
            //    Direction = Vector2.Zero;
            //    return true;
            //}
        }

        protected virtual void DirectionToOrientation()
        {
            if (Direction.Y > 0)
                Orientation = Orientation.Down;
            else if (Direction.Y < 0)
                Orientation = Orientation.Up;
            else if (Direction.X < 0)
                Orientation = Orientation.Left;
            else if (Direction.X > 0)
                Orientation = Orientation.Right;
        }

        protected virtual void OrientationToMirror()
        {
            switch (Orientation)
            {
                case Orientation.Left:
                    SpriteEffect = SpriteEffects.FlipHorizontally;
                    break;
                case Orientation.Right:
                    SpriteEffect = SpriteEffects.None;
                    break;
            }
        }
    }
}