﻿using System.Collections;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class SpatialGrid<T> : GlyphComponent, IGrid<T>
    {
        private readonly SceneNode _sceneNode;
        public IGrid<T> LocalGrid { get; set; }

        public bool IsVoid => LocalGrid.IsVoid;
        public TopLeftRectangle BoundingBox => LocalGrid.BoundingBox;
        public Vector2 Center => LocalGrid.Center;
        public GridDimension Dimension => LocalGrid.Dimension;
        public Rectangle Bounds => LocalGrid.Bounds;
        public Vector2 Delta => LocalGrid.Delta;
        public bool HasLowEntropy => LocalGrid.HasLowEntropy;
        public IEnumerable<T> Values => LocalGrid.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => LocalGrid.SignificantCases;

        public SpatialGrid(SceneNode sceneNode)
        {
            _sceneNode = sceneNode;
        }

        public T this[int i, int j] => LocalGrid[i, j];
        public T this[Point gridPoint] => LocalGrid[gridPoint];
        public T this[Vector2 worldPoint] => LocalGrid[worldPoint];
        public T this[IGridPositionable gridPositionable] => LocalGrid[gridPositionable];
        
        public bool ContainsPoint(Vector2 point) => LocalGrid.ContainsPoint(point);
        public bool ContainsPoint(int i, int j) => LocalGrid.ContainsPoint(i, j);
        public bool ContainsPoint(Point gridPoint) => LocalGrid.ContainsPoint(gridPoint);

        public bool Intersects(Segment segment) => LocalGrid.Intersects(segment);
        public bool Intersects<T1>(T1 edgedShape) where T1 : IEdgedShape => LocalGrid.Intersects(edgedShape);
        public bool Intersects(Circle circle) => LocalGrid.Intersects(circle);

        public Vector2 ToWorldPoint(int i, int j) => LocalGrid.ToWorldPoint(i, j);
        public Vector2 ToWorldPoint(Point gridPoint) => LocalGrid.ToWorldPoint(gridPoint);
        public Vector2 ToWorldPoint(IGridPositionable gridPoint) => LocalGrid.ToWorldPoint(gridPoint);
        public TopLeftRectangle ToWorldRange(int x, int y, int width, int height) => LocalGrid.ToWorldRange(x, y, width, height);
        public TopLeftRectangle ToWorldRange(Rectangle rectangle) => LocalGrid.ToWorldRange(rectangle);
        public Point ToGridPoint(Vector2 worldPoint) => LocalGrid.ToGridPoint(worldPoint);
        public Rectangle ToGridRange(TopLeftRectangle rectangle) => LocalGrid.ToGridRange(rectangle);

        public T[][] ToArray() => LocalGrid.ToArray();

        public IEnumerator<Point> GetEnumerator() => LocalGrid.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)LocalGrid).GetEnumerator();
    }
}