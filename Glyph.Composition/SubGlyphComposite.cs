using System.Collections;
using System.Collections.Generic;

namespace Glyph.Composition
{
    public class SubGlyphComposite<TComponent> : IList<TComponent>
        where TComponent : class, IGlyphComponent
    {
        private readonly IGlyphComposite<TComponent> _composite;
        private readonly IList<TComponent> _list;

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        public TComponent this[int index]
        {
            get => _list[index];
            set
            {
                _composite[GetCompositeIndex(index)] = value;
                _list[index] = value;
            }
        }

        public SubGlyphComposite(IGlyphComposite<TComponent> composite)
        {
            _composite = composite;
            _list = new List<TComponent>();
        }

        public void Add(TComponent item)
        {
            if (Count == 0)
                _composite.Add(item);
            else
                _composite.Insert(GetCompositeIndex(Count - 1) + 1, item);
            
            _list.Add(item);
        }

        public void Insert(int index, TComponent item)
        {
            if (index == _list.Count)
            {
                Add(item);
                return;
            }
            
            _composite.Insert(GetCompositeIndex(index), item);
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Remove(_list[index]);
        }

        public bool Remove(TComponent item) => _composite.Remove(item);
        public void Clear() => _composite.Clear();

        public void OnRemove(TComponent item)
        {
            _list.Remove(item);
        }
        
        public bool Contains(TComponent item)
        {
            return _list.Contains(item);
        }

        public int IndexOf(TComponent item)
        {
            return _list.IndexOf(item);
        }

        public void CopyTo(TComponent[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        private int GetCompositeIndex(int index) => _composite.IndexOf(_list[index]);

        public IEnumerator<TComponent> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}