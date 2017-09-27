using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public enum Orientation
    {
        Left,
        Right,
        Up,
        Down
    }

    [Flags]
    public enum Orientations
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
    }

    public enum Horizontal
    {
        Left,
        Right
    }

    [Flags]
    public enum Horizontals
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1
    }

    public enum Vertical
    {
        Up,
        Down
    }

    [Flags]
    public enum Verticals
    {
        None = 0,
        Up = 1 << 0,
        Down = 1 << 1
    }

    static public class OrientationExtensions
    {
        static public Orientation Opposite(this Orientation value)
        {
            switch (value)
            {
                case Orientation.Left: return Orientation.Right;
                case Orientation.Right: return Orientation.Left;
                case Orientation.Up: return Orientation.Down;
                case Orientation.Down: return Orientation.Up;
                default: throw new ArgumentException();
            }
        }

        static public Horizontal Opposite(this Horizontal value)
        {
            switch (value)
            {
                case Horizontal.Left: return Horizontal.Right;
                case Horizontal.Right: return Horizontal.Left;
                default: throw new ArgumentException();
            }
        }

        static public Vertical Opposite(this Vertical value)
        {
            switch (value)
            {
                case Vertical.Up: return Vertical.Down;
                case Vertical.Down: return Vertical.Up;
                default: throw new ArgumentException();
            }
        }

        static public Orientations Opposite(this Orientations flags)
        {
            var result = Orientations.None;
            if ((flags & Orientations.Left) != 0)
                result |= Orientations.Right;
            if ((flags & Orientations.Right) != 0)
                result |= Orientations.Left;
            if ((flags & Orientations.Up) != 0)
                result |= Orientations.Down;
            if ((flags & Orientations.Down) != 0)
                result |= Orientations.Up;
            return result;
        }

        static public Horizontals Opposite(this Horizontals flags)
        {
            var result = Horizontals.None;
            if ((flags & Horizontals.Left) != 0)
                result |= Horizontals.Right;
            if ((flags & Horizontals.Right) != 0)
                result |= Horizontals.Left;
            return result;
        }

        static public Verticals Opposite(this Verticals flags)
        {
            var result = Verticals.None;
            if ((flags & Verticals.Up) != 0)
                result |= Verticals.Down;
            if ((flags & Verticals.Down) != 0)
                result |= Verticals.Up;
            return result;
        }

        static public Orientations ToFlag(this Orientation value)
        {
            switch (value)
            {
                case Orientation.Left: return Orientations.Left;
                case Orientation.Right: return Orientations.Right;
                case Orientation.Up: return Orientations.Up;
                case Orientation.Down: return Orientations.Down;
                default: return Orientations.None;
            }
        }

        static public Horizontals ToFlag(this Horizontal value)
        {
            switch (value)
            {
                case Horizontal.Left: return Horizontals.Left;
                case Horizontal.Right: return Horizontals.Right;
                default: return Horizontals.None;
            }
        }

        static public Verticals ToFlag(this Vertical value)
        {
            switch (value)
            {
                case Vertical.Up: return Verticals.Up;
                case Vertical.Down: return Verticals.Down;
                default: return Verticals.None;
            }
        }

        static public IEnumerable<Orientation> ToValues(this Orientations flags)
        {
            if ((flags & Orientations.Left) != 0)
                yield return Orientation.Left;
            if ((flags & Orientations.Right) != 0)
                yield return Orientation.Right;
            if ((flags & Orientations.Up) != 0)
                yield return Orientation.Up;
            if ((flags & Orientations.Down) != 0)
                yield return Orientation.Down;
        }

        static public IEnumerable<Horizontal> ToValues(this Horizontals flags)
        {
            if ((flags & Horizontals.Left) != 0)
                yield return Horizontal.Left;
            if ((flags & Horizontals.Right) != 0)
                yield return Horizontal.Right;
        }

        static public IEnumerable<Vertical> ToValues(this Verticals flags)
        {
            if ((flags & Verticals.Up) != 0)
                yield return Vertical.Up;
            if ((flags & Verticals.Down) != 0)
                yield return Vertical.Down;
        }

        static public Orientations ToOrientations(this Vector2 vector)
        {
            var result = Orientations.None;
            if (vector.X < 0)
                result |= Orientations.Left;
            else if (vector.X > 0)
                result |= Orientations.Right;
            if (vector.Y < 0)
                result |= Orientations.Up;
            else if (vector.Y > 0)
                result |= Orientations.Down;
            return result;
        }

        static public Horizontal? ToHorizontal(this Vector2 vector)
        {
            if (vector.X < 0)
                return Horizontal.Left;
            if (vector.X > 0)
                return Horizontal.Right;
            return null;
        }

        static public Vertical? ToVertical(this Vector2 vector)
        {
            if (vector.Y < 0)
                return Vertical.Up;
            if (vector.Y > 0)
                return Vertical.Down;
            return null;
        }

        static public Vector2 ToVector(this Orientations flags)
        {
            Vector2 result = Vector2.Zero;
            if ((flags & Orientations.Left) != 0)
                result -= Vector2.UnitX;
            if ((flags & Orientations.Right) != 0)
                result += Vector2.UnitX;
            if ((flags & Orientations.Up) != 0)
                result -= -Vector2.UnitY;
            if ((flags & Orientations.Down) != 0)
                result += -Vector2.UnitY;
            return result.Normalized();
        }

        static public Vector2 ToVector(this Horizontal value)
        {
            switch (value)
            {
                case Horizontal.Left: return -Vector2.UnitX;
                case Horizontal.Right: return Vector2.UnitX;
                default: throw new ArgumentException();
            }
        }

        static public Vector2 ToVector(this Vertical value)
        {
            switch (value)
            {
                case Vertical.Up: return -Vector2.UnitY;
                case Vertical.Down: return Vector2.UnitY;
                default: throw new ArgumentException();
            }
        }
    }
}