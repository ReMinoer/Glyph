using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public class FilledCircleSprite : ShapedSpriteBase
    {
        public int Radius { get; set; }

        public FilledCircleSprite(Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(lazyGraphicsDevice)
        {
            Radius = 50;
        }

        protected override void GenerateTexture()
        {
            int outerDiameter = (Radius + 1) * 2;

            var data = new Color[outerDiameter * outerDiameter];
            
            for (int i = 0; i < outerDiameter; i++)
                for (int j = 0; j < outerDiameter; j++)
                {
                    Vector2 position = new Vector2(j, i).Substract(Radius);
                    data[i * outerDiameter + j] = position.Length() <= Radius ? Color : Color.Transparent;
                }

            _texture = new Texture2D(LazyGraphicsDevice.Value, outerDiameter, outerDiameter);
            Texture.SetData(data);
        }
    }
}