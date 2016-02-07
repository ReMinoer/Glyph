﻿using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IGrid : IShape
    {
        GridSize GridSize { get; }
        Vector2 Delta { get; }
        Vector2 ToWorldPoint(int i, int j);
        Vector2 ToWorldPoint(Point gridPoint);
        Vector2 ToWorldPoint(IGridPositionable gridPoint);
        Point ToGridPoint(Vector2 worldPoint);
        bool ContainsPoint(Point gridPoint);
    }

    public interface IGrid<out T> : IGrid
    {
        T this[int i, int j] { get; }
        T this[Point gridPoint] { get; }
        T this[Vector2 worldPoint] { get; }
        T this[IGridPositionable gridPositionable] { get; }
    }
}