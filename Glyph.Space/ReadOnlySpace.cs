using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class ReadOnlySpace : ISpace
    {
        private readonly ISpace _space;

        public bool IsVoid => _space.IsVoid;
        public TopLeftRectangle BoundingBox => _space.BoundingBox;
        public IEnumerable<Vector2> Points => _space.Points;
        public IEnumerable<TopLeftRectangle> Boxes => _space.Boxes;
        public IEnumerable<ISpace> Partitions => _space.Partitions;

        public ReadOnlySpace(ISpace space)
        {
            _space = space;
        }

        public bool ContainsPoint(Vector2 point) => _space.ContainsPoint(point);
        public bool Intersects(Segment segment) => _space.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => _space.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _space.Intersects(circle);
        public ISpace GetPartition(Vector2 position) => _space.GetPartition(position);
        public ISpace GetBestPartition(Vector2 position) => _space.GetBestPartition(position);
        public IEnumerable<ISpace> GetAllPartitionsInRange(TopLeftRectangle range) => _space.GetAllPartitionsInRange(range);
        public IEnumerable<Vector2> GetAllPointsInRange(IShape range) => _space.GetAllPointsInRange(range);
        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range) => _space.GetAllBoxesInRange(range);
    }

    public class ReadOnlySpace<T> : ISpace<T>
    {
        private readonly ISpace<T> _space;

        public bool IsVoid => _space.IsVoid;
        public TopLeftRectangle BoundingBox => _space.BoundingBox;
        public IEnumerable<Vector2> Points => _space.Points;
        public IEnumerable<TopLeftRectangle> Boxes => _space.Boxes;
        public IEnumerable<ISpace<T>> Partitions => _space.Partitions;
        IEnumerable<ISpace> ISpace.Partitions => ((ISpace)_space).Partitions;

        public ReadOnlySpace(ISpace<T> space)
        {
            _space = space;
        }

        public bool ContainsPoint(Vector2 point) => _space.ContainsPoint(point);
        public bool Intersects(Segment segment) => _space.Intersects(segment);
        public bool Intersects<TShape>(TShape edgedShape) where TShape : IEdgedShape => _space.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _space.Intersects(circle);
        public ISpace<T> GetBestPartition(Vector2 position) => _space.GetBestPartition(position);
        public IEnumerable<ISpace<T>> GetAllPartitionsInRange(TopLeftRectangle range) => _space.GetAllPartitionsInRange(range);
        public IEnumerable<T> GetAllItemsInRange(IShape range) => _space.GetAllItemsInRange(range);
        public ISpace<T> GetPartition(Vector2 position) => _space.GetPartition(position);
        public IEnumerable<Vector2> GetAllPointsInRange(IShape range) => _space.GetAllPointsInRange(range);
        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range) => _space.GetAllBoxesInRange(range);
        ISpace ISpace.GetPartition(Vector2 position) => ((ISpace)_space).GetPartition(position);
        ISpace ISpace.GetBestPartition(Vector2 position) => ((ISpace)_space).GetBestPartition(position);
        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(TopLeftRectangle range) => ((ISpace)_space).GetAllPartitionsInRange(range);
        public IEnumerator<T> GetEnumerator() => _space.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_space).GetEnumerator();
    }
}