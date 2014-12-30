using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class TransitionExtension
    {
        static public float Transition(this float value, float goal, float speed, GameTime gameTime)
        {
            float ecart = goal - value;
            if (!(Math.Abs(ecart) > float.Epsilon))
                return value;

            float modif = Math.Sign(ecart) * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Math.Abs(modif) < Math.Abs(ecart))
                return value + modif;

            return goal;
        }

        static public Vector2 Transition(this Vector2 value, Vector2 goal, float speed, GameTime gameTime)
        {
            Vector2 ecart = goal - value;
            if (ecart == Vector2.Zero)
                return value;

            Vector2 modif = Vector2.Normalize(ecart) * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (modif.Length() < ecart.Length())
                return value + modif;

            return goal;
        }
    }
}