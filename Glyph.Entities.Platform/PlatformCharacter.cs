using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Glyph.Entities.Platform
{
    public abstract class PlatformCharacter : GameObject, ILayable
    {
        protected Collision Collision;
        // BUG : Correction de position quand changement de sens
        // TODO : Reflechir à la sauvegarde des perso (jumelle, AI,...)
        // WATCH : Cohésion du sol sur longue chute

        [XmlIgnore]
        public virtual float VerticalSpeed { get; set; }

        [XmlIgnore]
        public virtual bool OnGround { get; set; }

        public virtual int Layer { get; set; }

        [XmlIgnore]
        public override Rectangle Hitbox
        {
            get
            {
                var hitbox = new Rectangle();
                if (Orientation == Orientation.Left)
                    hitbox.X = (int)Position.X + (RectangleSource.Width - (CollisionMask.X + CollisionMask.Width));
                else
                    hitbox.X = (int)Position.X + CollisionMask.X;
                hitbox.Y = (int)Position.Y + CollisionMask.Y;
                hitbox.Width = CollisionMask.Width;
                hitbox.Height = CollisionMask.Height;
                return hitbox;
            }
        }

        //[XmlIgnore]
        //public override Vector2 Centre
        //{
        //    get
        //    {
        //        Vector2 centre = Vector2.Zero;
        //        centre.X = Hitbox.X + (Hitbox.Width * Agrandissement) / 2;
        //        centre.Y = Hitbox.Y + (Hitbox.Height * Agrandissement) / 2;
        //        return centre;
        //    }
        //    set
        //    {
        //        Vector2 temp = Vector2.Zero;
        //        temp.X = value.X - (Hitbox.X - Position.X) - (Hitbox.Width * Agrandissement) / 2;
        //        temp.Y = value.Y - (Hitbox.Y - Position.Y) - (Hitbox.Height * Agrandissement) / 2;
        //        Position = temp;
        //    }
        //}

        protected abstract Rectangle CollisionMask { get; }

        public override void Initialize()
        {
            base.Initialize();

            VerticalSpeed = 0;
            OnGround = false;
            Orientation = Orientation.Right;
        }

        public virtual void Update(GameTime gameTime, IPlatformerSpace space)
        {
            if (!OnGround)
                VerticalSpeed += space.Gravity * (float)(gameTime.ElapsedGameTime.TotalSeconds);

            Position += VerticalSpeed * Vector2.UnitY * (float)(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);

            ManageCollision(gameTime, space);
        }

        public ObstructionType CollisionRectangle(Rectangle rect)
        {
            if (!Hitbox.Impact(rect))
                return ObstructionType.None;

            ObstructionType type;

            int top = Math.Max(Hitbox.Top, rect.Top);
            int bottom = Math.Min(Hitbox.Bottom, rect.Bottom);
            int left = Math.Max(Hitbox.Left, rect.Left);
            int right = Math.Min(Hitbox.Right, rect.Right);

            if (Hitbox.Top <= rect.Top && right - left > bottom - top)
                type = ObstructionType.Floor;
            else if (Hitbox.Bottom >= rect.Bottom && right - left > bottom - top)
                type = ObstructionType.Ceiling;
            else if (Hitbox.Right >= rect.Right)
                type = ObstructionType.WallLeft;
            else
                type = ObstructionType.WallRight;

            TraitementObstacle(type, right - left, bottom - top);

            return type;
        }

        public ObstructionType CollisionObject(GameObject obj)
        {
            ObstructionType type = CollisionRectangle(obj.Hitbox);

            if (type != ObstructionType.Floor)
                return type;

            var dynamicFloor = obj as IDynamicFloor;
            if (dynamicFloor == null)
                return type;

            if (Math.Abs(dynamicFloor.DynamicX) > float.Epsilon)
                Collision.NewDynamiqueX = dynamicFloor.DynamicX;
            if (Math.Abs(dynamicFloor.DynamicY) > float.Epsilon)
                Collision.NewDynamiqueY = dynamicFloor.DynamicY;

            return type;
        }

        public ObstructionType CollisionBorders(Rectangle rect)
        {
            if (Hitbox.X < rect.X)
            {
                TraitementObstacle(ObstructionType.WallLeft, rect.X - Hitbox.X, 0);
                return ObstructionType.WallLeft;
            }

            if (Hitbox.Right >= rect.Right)
            {
                TraitementObstacle(ObstructionType.WallRight, Hitbox.Right - rect.Right, 0);
                return ObstructionType.WallRight;
            }

            return ObstructionType.None;
        }

        protected virtual void ManageCollision(GameTime gameTime, IPlatformerSpace space)
        {
            Collision = new Collision();

            space.ManageCollision(this);

            if (Collision.IsCeiling && Collision.IsFloor)
            {
                Position = Position.Substract(100, 0);
                VerticalSpeed = 0;
            }

            if (Collision.IsWallLeft && Collision.IsWallRight)
            {
                Position = Position.Substract(0, 100);
                VerticalSpeed = 0;
            }

            if (Collision.IsCeiling && Collision.IsFloor)
            {
                Position = Position.Substract(100, 0);
                VerticalSpeed = 0;
            }

            if (Collision.IsFloor && VerticalSpeed >= 0)
            {
                Position =
                    Position.SetY(Collision.NewPosY).
                        Add(Collision.NewDynamiqueX * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0);
                VerticalSpeed = Collision.NewDynamiqueY;
                OnGround = true;
            }
            else
                OnGround = false;

            if (Collision.IsCeiling && VerticalSpeed < 0)
            {
                Position = Position.SetY(Collision.NewPosY);
                VerticalSpeed = 0;
            }

            if (Collision.IsWallLeft || Collision.IsWallRight)
                Position = Position.SetX(Collision.NewPosX);
        }

        protected void TraitementObstacle(ObstructionType type, int w, int h)
        {
            switch (type)
            {
                case ObstructionType.Floor:
                    Collision.IsFloor = true;
                    if (Math.Abs(Collision.NewPosY) < float.Epsilon)
                        Collision.NewPosY = Position.Y - (h - 1);
                    break;

                case ObstructionType.Ceiling:
                    Collision.IsCeiling = true;
                    if (Math.Abs(Collision.NewPosY) < float.Epsilon)
                        Collision.NewPosY = Position.Y + (h - 1);
                    break;

                case ObstructionType.WallLeft:
                    Collision.IsWallLeft = true;
                    if (Math.Abs(Collision.NewPosX) < float.Epsilon)
                        Collision.NewPosX = Position.X + (w - 1);
                    break;

                case ObstructionType.WallRight:
                    Collision.IsWallRight = true;
                    if (Math.Abs(Collision.NewPosX) < float.Epsilon)
                        Collision.NewPosX = Position.X - (w - 1);
                    break;
            }
        }

        protected override void DirectionToOrientation()
        {
            if (Direction.X < 0)
                Orientation = Orientation.Left;
            else if (Direction.X > 0)
                Orientation = Orientation.Right;
        }
    }
}