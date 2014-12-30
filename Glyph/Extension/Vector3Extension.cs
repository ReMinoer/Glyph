﻿using Microsoft.Xna.Framework;

namespace Glyph.Extension
{
    static public class Vector3Extension
    {
        static public Vector2 XY(this Vector3 value)
        {
            return new Vector2(value.X, value.Y);
        }

        static public Vector2 XZ(this Vector3 value)
        {
            return new Vector2(value.X, value.Z);
        }

        static public Vector2 YZ(this Vector3 value)
        {
            return new Vector2(value.Y, value.Z);
        }
    }
}