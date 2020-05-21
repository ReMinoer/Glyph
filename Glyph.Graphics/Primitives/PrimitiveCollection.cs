using System.Collections;
using System.Collections.Generic;

namespace Glyph.Graphics.Primitives
{
    public class PrimitiveCollection : IPrimitiveProvider, IList<IPrimitive>
    {
        private readonly IList<IPrimitive> _list = new List<IPrimitive>();
        public IEnumerable<IPrimitive> Primitives => _list;

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                foreach (IPrimitive primitive in _list)
                    primitive.Visible = value;
            }
        }

        public IPrimitive this[int index]
        {
            get => _list[index];
            set
            {
                value.Visible = Visible;
                _list[index] = value;
            }
        }

        public int Count => _list.Count;

        public void Add(IPrimitive item)
        {
            item.Visible = Visible;
            _list.Add(item);
        }

        public void Insert(int index, IPrimitive item)
        {
            item.Visible = Visible;
            _list.Insert(index, item);
        }

        public bool Remove(IPrimitive item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Clear() => _list.Clear();
        public bool Contains(IPrimitive item) => _list.Contains(item);
        public int IndexOf(IPrimitive item) => _list.IndexOf(item);
        public void CopyTo(IPrimitive[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        bool ICollection<IPrimitive>.IsReadOnly => _list.IsReadOnly;

        public IEnumerator<IPrimitive> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}