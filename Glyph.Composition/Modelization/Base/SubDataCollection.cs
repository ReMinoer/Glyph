using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Diese.Collections.Observables;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization.Base
{
    public class SubDataCollection<TGlyphData> : IObservableCollection<TGlyphData>
        where TGlyphData : IGlyphData
    {
        private readonly IDependencyResolverClient _parentData;
        private readonly IObservableCollection<TGlyphData> _collectionImplementation;

        public int Count => _collectionImplementation.Count;
        bool ICollection<TGlyphData>.IsReadOnly => _collectionImplementation.IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SubDataCollection(IDependencyResolverClient parentData)
        {
            _parentData = parentData;

            _collectionImplementation = new ObservableList<TGlyphData>();
            _collectionImplementation.PropertyChanged += OnPropertyChanged;
            _collectionImplementation.CollectionChanged += OnCollectionChanged;
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(sender, e);

        public void Add(TGlyphData item)
        {
            _collectionImplementation.Add(item);
        }

        public bool Remove(TGlyphData item) => _collectionImplementation.Remove(item);
        public void Clear() => _collectionImplementation.Clear();
        public bool Contains(TGlyphData item) => _collectionImplementation.Contains(item);
        public void CopyTo(TGlyphData[] array, int arrayIndex) => _collectionImplementation.CopyTo(array, arrayIndex);
        public IEnumerator<TGlyphData> GetEnumerator() => _collectionImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collectionImplementation).GetEnumerator();
    }
}