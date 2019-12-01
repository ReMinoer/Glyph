using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Diese.Collections.Observables;

namespace Glyph.Composition.Modelization.Base
{
    public class SubDataCollection<TGlyphData, TData, T> : IObservableCollection<TGlyphData>
        where TGlyphData : IGlyphData
        where T : class, IGlyphComponent
        where TData : BindedData<TData, T>
    {
        private readonly IObservableCollection<TGlyphData> _collectionImplementation;
        private readonly BindedData<TData, T> _bindedData;

        public int Count => _collectionImplementation.Count;
        bool ICollection<TGlyphData>.IsReadOnly => _collectionImplementation.IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SubDataCollection(BindedData<TData, T> bindedData)
        {
            _collectionImplementation = new ObservableCollection<TGlyphData>();
            _bindedData = bindedData;

            _collectionImplementation.PropertyChanged += OnPropertyChanged;
            _collectionImplementation.CollectionChanged += OnCollectionChanged;
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(sender, e);

        public void Add(TGlyphData item)
        {
            _collectionImplementation.Add(item);
            item.DependencyResolver = _bindedData.Resolver;
        }

        public bool Remove(TGlyphData item) => _collectionImplementation.Remove(item);
        public void Clear() => _collectionImplementation.Clear();
        public bool Contains(TGlyphData item) => _collectionImplementation.Contains(item);
        public void CopyTo(TGlyphData[] array, int arrayIndex) => _collectionImplementation.CopyTo(array, arrayIndex);
        public IEnumerator<TGlyphData> GetEnumerator() => _collectionImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collectionImplementation).GetEnumerator();
    }
}