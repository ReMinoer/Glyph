using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    static public class SpriteBatchExtension
    {
        static public void DrawRepeat(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle,
                                      Color color)
        {
            DrawRepeat(spriteBatch, texture, destinationRectangle, null, color);
        }

        static public void DrawRepeat(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle,
                                      Rectangle? sourceRectangle, Color color)
        {
            DrawRepeat(spriteBatch, texture, destinationRectangle, sourceRectangle, Angle.Rotation.None, color);
        }

        static public void DrawRepeat(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle,
                                      Rectangle? sourceRectangle, Angle.Rotation rotation, Color color)
        {
            Rectangle source = sourceRectangle.HasValue
                                   ? sourceRectangle.Value
                                   : new Rectangle(0, 0, texture.Width, texture.Height);

            bool reverse = (Math.Abs(rotation.Value - Angle.Rotation.RotateLeft.Value) < float.Epsilon
                            || Math.Abs(rotation.Value - Angle.Rotation.RotateRight.Value) < float.Epsilon);

            int rotateWidth = reverse ? source.Height : source.Width;
            int rotateHeight = reverse ? source.Width : source.Height;

            for (int x = 0; x < destinationRectangle.Width; x += rotateWidth)
                for (int y = 0; y < destinationRectangle.Height; y += rotateHeight)
                {
                    int w = destinationRectangle.Width - x;
                    if (w > rotateWidth)
                        w = rotateWidth;

                    int h = destinationRectangle.Height - y;
                    if (h > rotateHeight)
                        h = rotateHeight;

                    var drawSource = new Rectangle(source.X, source.Y, reverse ? h : w, reverse ? w : h);

                    Vector2 position = Vector2.Zero;
                    if (Math.Abs(rotation.Value - Angle.Rotation.None.Value) < float.Epsilon)
                        position = destinationRectangle.Origin().ToVector2().Add(x, y);
                    else if (Math.Abs(rotation.Value - Angle.Rotation.RotateLeft.Value) < float.Epsilon)
                        position = destinationRectangle.P1().ToVector2().Add(-x, y);
                    else if (Math.Abs(rotation.Value - Angle.Rotation.RotateRight.Value) < float.Epsilon)
                        position = destinationRectangle.Origin().ToVector2().Add(x, y + source.Width);
                    else if (Math.Abs(rotation.Value - Angle.Rotation.RotateOpposite.Value) < float.Epsilon)
                        position = destinationRectangle.P2().ToVector2().Add(x + source.Width, -h);

                    spriteBatch.Draw(texture, position, drawSource, color, rotation, Vector2.Zero, 1f,
                        SpriteEffects.None, 0);
                }
        }
    }
}