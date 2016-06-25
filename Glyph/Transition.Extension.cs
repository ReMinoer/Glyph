using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class TransitionExtension
    {
        static public float Transition(this float value, float goal, float speed, GameTime gameTime)
        {
            return Transition(value, goal, speed, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        static public Vector2 Transition(this Vector2 value, Vector2 goal, float speed, GameTime gameTime)
        {
            return Transition(value, goal, speed, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        static public float Transition(this float value, float goal, float speed, float deltaTime)
        {
            float ecart = goal - value;
            if (!(System.Math.Abs(ecart) > float.Epsilon))
                return value;

            float modif = System.Math.Sign(ecart) * speed * deltaTime;
            if (System.Math.Abs(modif) < System.Math.Abs(ecart))
                return value + modif;

            return goal;
        }

        static public Vector2 Transition(this Vector2 value, Vector2 goal, float speed, float deltaTime)
        {
            Vector2 ecart = goal - value;
            if (ecart == Vector2.Zero)
                return value;

            Vector2 modif = Vector2.Normalize(ecart) * speed * deltaTime;
            if (modif.Length() < ecart.Length())
                return value + modif;

            return goal;
        }
    }
}