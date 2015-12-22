using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Modelization;

namespace Glyph.Composition
{
    public class Contractor<T> : IList<T>, ICreator<T>
        where T : class, IGlyphComponent
    {
        private readonly GlyphSchedulableBase _parent;
        private readonly List<WeakReference<T>> _list;

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                T obj;
                _list[index].TryGetTarget(out obj);
                return obj;
            }
            set
            {
                if (index < Count)
                {
                    T obj;
                    _list[index].TryGetTarget(out obj);
                    _parent.Remove(obj);
                }

                _parent.Add(value);
                _list[index].SetTarget(value);
            }
        }

        public Contractor(GlyphSchedulableBase parent)
        {
            _parent = parent;
            _list = new List<WeakReference<T>>();
        }

        public void Init(IEnumerable<T> items)
        {
            Clear();

            foreach (T item in items)
                Add(item);
        }

        public T Create()
        {
            var item = _parent.Add<T>();
            _list.Add(new WeakReference<T>(item));
            return item;
        }

        public void Add(T item)
        {
            _parent.Add(item);
            _list.Add(new WeakReference<T>(item));
        }

        public void Clear()
        {
            foreach (WeakReference<T> weakReference in _list)
            {
                T obj;
                weakReference.TryGetTarget(out obj);
                _parent.Remove(obj);
            }

            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Any(x =>
            {
                T obj;
                x.TryGetTarget(out obj);
                return obj == item;
            });
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = this[i];
        }

        public bool Remove(T item)
        {
            _parent.Remove(item);
            return _list.Remove(new WeakReference<T>(item));
        }

        public int IndexOf(T item)
        {
            return _list.FindIndex(x =>
            {
                T obj;
                x.TryGetTarget(out obj);
                return obj == item;
            });
        }

        public void Insert(int index, T item)
        {
            _parent.Add(item);
            _list.Insert(index, new WeakReference<T>(item));
        }

        public void RemoveAt(int index)
        {
            WeakReference<T> weakReference = _list[index];

            T obj;
            weakReference.TryGetTarget(out obj);
            _parent.Remove(obj);

            _list.RemoveAt(index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Select(x =>
            {
                T obj;
                x.TryGetTarget(out obj);
                return obj;
            })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}