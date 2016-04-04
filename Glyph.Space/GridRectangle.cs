using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class GridRectangle : IGrid
    {
        public IRectangle Rectangle { get; private set; }
        public GridSize GridSize { get; private set; }
        public Vector2 Delta { get; private set; }

        public Vector2 Center
        {
            get { return Rectangle.Center; }
            set { Rectangle.Center = value; }
        }

        public GridRectangle(IRectangle rectangle, int columns, int rows)
        {
            Rectangle = rectangle;
            GridSize = new GridSize(columns, rows);

            Delta = Rectangle.Size / GridSize;
        }

        public IRectangle BoundingBox
        {
            get { return Rectangle; }
        }

        public bool ContainsPoint(Vector2 worldPoint)
        {
            return Rectangle.ContainsPoint(worldPoint);
        }

        public bool ContainsPoint(Point gridPoint)
        {
            return gridPoint.X >= 0 && gridPoint.X < GridSize.Columns && gridPoint.Y >= 0 && gridPoint.Y < GridSize.Rows;
        }

        public Vector2 ToWorldPoint(int i, int j)
        {
            return ToWorldPoint(new Point(j, i));
        }

        public Vector2 ToWorldPoint(Point gridPoint)
        {
            return Rectangle.Origin + Delta.Integrate(gridPoint);
        }

        public Vector2 ToWorldPoint(IGridPositionable gridPositionable)
        {
            return ToWorldPoint(gridPositionable.GridPosition);
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            return Delta.Discretize(worldPoint - Rectangle.Origin);
        }

        public bool Intersects(IRectangle rectangle)
        {
            return Rectangle.Intersects(rectangle);
        }

        public bool Intersects(ICircle circle)
        {
            return Rectangle.Intersects(circle);
        }
    }

    public class GridRectangle<T> : GridRectangle, IGrid<T>
    {
        private readonly T[][] _data;

        public GridRectangle(IRectangle rectangle, int columns, int rows)
            : base(rectangle, columns, rows)
        {
            _data = new T[GridSize.Rows][];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = new T[GridSize.Columns];
        }

        public virtual T this[int i, int j]
        {
            get { return _data[i][j]; }
        }

        public T this[Point gridPoint]
        {
            get { return this[gridPoint.Y, gridPoint.X]; }
        }

        public T this[Vector2 worldPoint]
        {
            get { return this[ToGridPoint(worldPoint)]; }
        }

        public T this[IGridPositionable gridPositionable]
        {
            get { return this[gridPositionable.GridPosition]; }
        }
    }
}