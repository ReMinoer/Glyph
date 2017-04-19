using System;
using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class Space<T> : WriteableSpaceBase<T>
    {
        private readonly List<T> _items;
        protected override ICollection<T> Items => _items;

        public Space(Func<T, Vector2> getPoint, IPartitioner partitioner = null)
            : this(null, getPoint, x => new CenteredRectangle(getPoint(x), 1, 1), partitioner)
        {
        }

        public Space(Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(null, x => getBox(x).Center, getBox, partitioner)
        {
        }

        public Space(Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(null, getPoint, getBox, partitioner)
        {
        }

        protected Space(WriteableSpaceBase<T> parent, Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : base(parent, getPoint, getBox, partitioner)
        {
            _items = new List<T>();
        }

        protected override IWriteableSpace<T> CreatePartition(IPartitioner partitioner)
        {
            return new Space<T>(this, GetPoint, GetBox, partitioner);
        }
    }
}