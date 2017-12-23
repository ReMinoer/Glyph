using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Binding
{
    public class KeyableCollection<T, TKey> : IKeyableCollection<T, TKey>
    {
        private readonly Dictionary<TKey, T> _keyed = new Dictionary<TKey, T>();
        private readonly List<T> _unkeyed = new List<T>();
        
        public int Count => _keyed.Count + _unkeyed.Count;
        bool ICollection<T>.IsReadOnly => false;
        
        public T this[TKey key] => _keyed[key];
        public bool TryGetBinding(TKey key, out T item) => _keyed.TryGetValue(key, out item);

        public void Add(T item)
        {
            _unkeyed.Add(item);
        }

        public void Add(T item, TKey key)
        {
            if (key != null)
                _keyed.Add(key, item);
            else
                _unkeyed.Add(item);
        }

        public bool Remove(T item) => _keyed.ContainsValue(item) ? _keyed.Remove(_keyed.First(x => object.Equals(x.Value, item)).Key) : _unkeyed.Remove(item);

        public void Clear()
        {
            _keyed.Clear();
            _unkeyed.Clear();
        }

        public bool Contains(T item)
        {
            return _keyed.ContainsValue(item) || _unkeyed.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _keyed.Values.CopyTo(array, arrayIndex);
            _unkeyed.CopyTo(array, arrayIndex + _keyed.Count);
        }

        public IEnumerator<T> GetEnumerator() => Enumerable.Concat(_keyed.Values, _unkeyed).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_unkeyed).GetEnumerator();
    }

    public interface IKeyableCollection<T, in TKey> : IReadOnlyKeyableCollection<T, TKey>, ICollection<T>
    {
        void Add(T item, TKey key);
    }

    public interface IReadOnlyKeyableCollection<T, in TKey> : IReadOnlyCollection<T>
    {
        T this[TKey key] { get; }
        bool TryGetBinding(TKey key, out T item);
    }

    public class ReadOnlyKeyableCollection<T, TKey> : IReadOnlyKeyableCollection<T, TKey>
    {
        private readonly KeyableCollection<T, TKey> _keyableCollection;
        public int Count => _keyableCollection.Count;

        public ReadOnlyKeyableCollection(KeyableCollection<T, TKey> keyableCollection)
        {
            _keyableCollection = keyableCollection;
        }

        public T this[TKey key] => _keyableCollection[key];
        public bool TryGetBinding(TKey key, out T item) => _keyableCollection.TryGetBinding(key, out item);

        public IEnumerator<T> GetEnumerator() => _keyableCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_keyableCollection).GetEnumerator();
    }

    public class OneSideBindingCollection<TModel, TView> : KeyableCollection<IOneSideBinding<TModel, TView>, string>
    {
    }
}