using System;
using System.Collections.Generic;
using Glyph.Composition;
using Simulacra;

namespace Glyph.Core.Tracking
{
    public class Factory<T> : DisposableTracker<T>, ICreator<T>
        where T : class, IGlyphComponent
    {
        private readonly IGlyphCompositeResolver _parent;
        public event Action<T> ComponentCreated;

        public Factory(IGlyphCompositeResolver parent)
        {
            _parent = parent;
        }

        public void Init(IEnumerable<T> items)
        {
            Clear();

            foreach (T item in items)
                Register(item);
        }

        public T Create()
        {
            var item = _parent.Add<T>();
            ComponentCreated?.Invoke(item);

            base.Register(item);
            return item;
        }

        public override bool Register(T item)
        {
            if (!base.Register(item))
                return false;

            _parent.Add(item);
            return true;
        }

        public override bool Unregister(T item)
        {
            if (!base.Unregister(item))
                return false;

            _parent.Remove(item);
            return true;
        }

        public override void Clear()
        {
            foreach (T item in this)
                _parent.Remove(item);

            base.Clear();
        }
    }
}