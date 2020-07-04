using System;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class TrackingSpace<T> : WriteableSpaceBase<T>
        where T : class, INotifyDisposed
    {
        private readonly DisposableTracker<T> _tracker;
        protected override ICollection<T> Items => _tracker;

        public TrackingSpace(Func<T, Vector2> getPoint, IPartitioner partitioner = null)
            : this(null, getPoint, x => new CenteredRectangle(getPoint(x), 1, 1), partitioner)
        {
        }

        public TrackingSpace(Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(null, x => getBox(x).Center, getBox, partitioner)
        {
        }

        public TrackingSpace(Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(null, getPoint, getBox, partitioner)
        {
        }

        protected TrackingSpace(WriteableSpaceBase<T> parent, Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : base(parent, getPoint, getBox, partitioner)
        {
            _tracker = new DisposableTracker<T>();
        }

        protected override IWriteableSpace<T> CreatePartition(IPartitioner partitioner)
        {
            return new TrackingSpace<T>(this, GetPoint, GetBox, partitioner);
        }
    }
}