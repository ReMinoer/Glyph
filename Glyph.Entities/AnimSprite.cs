using System.Collections.Generic;
using System.Xml.Serialization;
using Diese.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Entities
{
    public class AnimSprite : Sprite
    {
        [XmlIgnore]
        public Texture2DSplit TextureSplit { get; set; }

        public SerializableDictionary<string, Animation> Animations { get; set; }
        public virtual Point NbFrames { get; set; }
        public string CurrentAnimation { get; set; }

        [XmlIgnore]
        public virtual int CurrentFrame { get; set; }

        [XmlIgnore]
        public override Texture2D Texture
        {
            get { return TextureSplit.IsEmpty ? null : TextureSplit[0]; }
        }

        [XmlIgnore]
        public virtual Vector2 FrameSize
        {
            get { return TextureSplit[0].Size() / NbFrames.ToVector2(); }
        }

        [XmlIgnore]
        public virtual int TextureTarget
        {
            get
            {
                var totalFrames = 0;
                for (var i = 0; i < TextureSplit.Count; i++)
                {
                    Point framesXY = NbFramesPerTexture(i);
                    totalFrames += framesXY.X * framesXY.Y;
                    if (CurrentFrame < totalFrames)
                        return i;
                }
                return 0;
            }
        }

        [XmlIgnore]
        public override Rectangle RectangleSource
        {
            get
            {
                var rectangleSource = new Rectangle();
                var totalFrames = 0;
                for (var i = 0; i < TextureSplit.Count; i++)
                {
                    Point framesXY = NbFramesPerTexture(i);
                    totalFrames += framesXY.X * framesXY.Y;

                    if (CurrentFrame >= totalFrames)
                        continue;

                    rectangleSource.X = ((CurrentFrame - totalFrames + framesXY.X * framesXY.Y) % framesXY.X)
                                        * (int)FrameSize.X;
                    rectangleSource.Y = ((CurrentFrame - totalFrames + framesXY.X * framesXY.Y) / framesXY.X)
                                        * (int)FrameSize.Y;
                    rectangleSource.Width = (int)FrameSize.X;
                    rectangleSource.Height = (int)FrameSize.Y;
                    break;
                }
                return rectangleSource;
            }
        }

        public AnimSprite()
        {
            TextureSplit = new Texture2DSplit();
            Animations = new SerializableDictionary<string, Animation>{{"defaut", Animation.Default}};
            CurrentAnimation = "defaut";
        }

        public override void Initialize()
        {
            base.Initialize();

            CurrentFrame = 0;
            SpriteEffect = SpriteEffects.None;
        }

        public override void LoadContent(ContentLibrary ressources)
        {
            LoadContent(ressources, Asset);
        }

        public virtual void LoadContent(ContentLibrary ressources, string name, int frameX = 1, int frameY = 1)
        {
            Asset = name;
            NbFrames = new Point(frameX, frameY);
            TextureSplit.LoadContent(ressources, Asset);
        }

        public override void Update(GameTime gameTime)
        {
            Animations[CurrentAnimation].Update(gameTime);
            CurrentFrame = Animations[CurrentAnimation].FrameActual;

            base.Update(gameTime);

            UpdateAnimation(gameTime);
        }

        public void ChangeAnimation(string anim)
        {
            if (anim == CurrentAnimation || !Animations[CurrentAnimation].CanChange || !Animations.ContainsKey(anim))
                return;

            CurrentAnimation = anim;
            Animations[CurrentAnimation].Initialize();
            CurrentFrame = Animations[CurrentAnimation].FrameActual;
        }

        public Point NbFramesPerTexture(int idTexture)
        {
            if (idTexture == 0)
                return NbFrames;
            return new Point((int)(TextureSplit[idTexture].Width / FrameSize.X),
                (int)(TextureSplit[idTexture].Height / FrameSize.Y));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!TextureSplit.IsEmpty)
                spriteBatch.Draw(TextureSplit[TextureTarget], PositionScreen, RectangleSource, Color * Opacity, Rotation,
                    Origin, Scale, SpriteEffect, 0);
        }

        protected virtual void UpdateAnimation(GameTime gameTime)
        {
        }
    }
}