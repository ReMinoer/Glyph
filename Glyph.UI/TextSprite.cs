using System.Xml.Serialization;
using Glyph.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.UI
{
    public class TextSprite : Sprite
    {
        [XmlIgnore]
        public virtual SpriteFont Font { get; set; }
        public virtual string Text { get; set; }
        public virtual int Contour { get; set; }
        public virtual Color ContourColor { get; set; }
        public virtual float ContourOpacity { get; set; }
        public virtual Vector2 Shadow { get; set; }
        public virtual Color ShadowColor { get; set; }
        public virtual float ShadowOpacity { get; set; }
        public virtual int ShadowGradient { get; set; }
        [XmlIgnore]
        public override Rectangle RectangleSource
        {
            get
            {
                var rectangleSource = new Rectangle();
                Vector2 taille = Font.MeasureString(Text);
                rectangleSource.X = 0;
                rectangleSource.Y = 0;
                rectangleSource.Width = (int)taille.X;
                rectangleSource.Height = (int)taille.Y;
                return rectangleSource;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Text = "";

            Contour = 0;
            ContourColor = Color.Black;
            ContourOpacity = Opacity;

            Shadow = Vector2.Zero;
            ShadowColor = Color.Black;
            ShadowGradient = 0;
            ShadowOpacity = Opacity;
        }

        public virtual void Initialize(string txt, int x, int y)
        {
            Initialize();
            Text = txt;
            Position = new Vector2(x, y);
        }

        public override void LoadContent(ContentLibrary ressources)
        {
            if (Asset != "")
                Font = ressources.GetFont(Asset);
        }

        public override void LoadContent(ContentLibrary ressources, string asset)
        {
            Asset = asset;
            if (Asset != "")
                Font = ressources.GetFont(asset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Font == null || Text == null)
                return;

            if (Shadow != Vector2.Zero || ShadowGradient != 0)
                for (int i = -Contour; i <= Contour; i++)
                    for (int j = -Contour; j <= Contour; j++)
                        spriteBatch.DrawString(Font, Text, PositionScreen + Shadow + new Vector2(i, j),
                            ShadowColor * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //for (int i = ShadowGradient; i > 0; i--)
            //{
            //    Vector2 x = i * Vector2.UnitX;
            //    Vector2 y = i * Vector2.UnitY;

            //    spriteBatch.DrawString(Font, Text, PositionScreen + Shadow - x - y, ShadowColor * (ShadowOpacity / ShadowGradient) * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen + Shadow - x + y, ShadowColor * (ShadowOpacity / ShadowGradient) * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen + Shadow + x - y, ShadowColor * (ShadowOpacity / ShadowGradient) * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen + Shadow + x + y, ShadowColor * (ShadowOpacity / ShadowGradient) * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //}
            //spriteBatch.DrawString(Font, Text, PositionScreen + Shadow, ShadowColor * ShadowOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);

            //for (int i = -ShadowGradient; i <= ShadowGradient; i++)
            //    for (int j = -ShadowGradient; j <= ShadowGradient; j++)
            //    {
            //        float op = ShadowOpacity / (ShadowGradient);
            //        spriteBatch.DrawString(Font, Text, PositionScreen + Shadow + new Vector2(i, j), ShadowColor * op * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    }

            if (Contour != 0)
                for (int i = -Contour; i <= Contour; i++)
                    for (int j = -Contour; j <= Contour; j++)
                        spriteBatch.DrawString(Font, Text, PositionScreen + new Vector2(i, j),
                            ContourColor * ContourOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);

            //for (int i = 1; i <= Contour; i++)
            //{
            //    Vector2 x = i * Vector2.UnitX;
            //    Vector2 y = i * Vector2.UnitY;

            //    spriteBatch.DrawString(Font, Text, PositionScreen - x - y, ContourColor * ContourOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen - x + y, ContourColor * ContourOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen + x - y, ContourColor * ContourOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //    spriteBatch.DrawString(Font, Text, PositionScreen + x + y, ContourColor * ContourOpacity * Opacity, Rotation, Origin, Scale, SpriteEffects.None, 0);
            //}

            spriteBatch.DrawString(Font, Text, PositionScreen, Color * Opacity, Rotation, Origin, Scale,
                SpriteEffects.None, 0);
        }

        public Vector2 MeasureString()
        {
            return Font.MeasureString(Text);
        }

        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }

        public override string ToString()
        {
            return "\"" + Text + "\"";
        }
    }
}