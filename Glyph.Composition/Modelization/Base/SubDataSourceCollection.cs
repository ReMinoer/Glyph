using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization.Base
{
    public class SubDataSourceCollection<TGlyphData> : IObservableCollection<IReadOnlyObservableCollection<TGlyphData>>
        where TGlyphData : class, IGlyphData
    {
        private readonly IObservableCollection<IReadOnlyObservableCollection<TGlyphData>> _collectionImplementation;
        private readonly IDependencyResolverClient _parentData;

        public int Count => _collectionImplementation.Count;
        bool ICollection<IReadOnlyObservableCollection<TGlyphData>>.IsReadOnly => _collectionImplementation.IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SubDataSourceCollection(IDependencyResolverClient parentData)
        {
            _collectionImplementation = new ObservableCollection<IReadOnlyObservableCollection<TGlyphData>>();
            _parentData = parentData;

            _collectionImplementation.PropertyChanged += OnPropertyChanged;
            _collectionImplementation.CollectionChanged += OnCollectionChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (IReadOnlyObservableCollection<TGlyphData> newItem in e.NewItems)
                foreach (TGlyphData subItem in newItem)
                    subItem.DependencyResolver = _parentData.DependencyResolver;

            CollectionChanged?.Invoke(sender, e);
        }

        public void Add(IReadOnlyObservableCollection<TGlyphData> item)
        {
            _collectionImplementation.Add(item);

            if (item is null)
                return;

            foreach (TGlyphData subItem in item)
                subItem.DependencyResolver = _parentData.DependencyResolver;
        }

        public bool Remove(IReadOnlyObservableCollection<TGlyphData> item) => _collectionImplementation.Remove(item);
        public void Clear() => _collectionImplementation.Clear();
        public bool Contains(IReadOnlyObservableCollection<TGlyphData> item) => _collectionImplementation.Contains(item);
        public void CopyTo(IReadOnlyObservableCollection<TGlyphData>[] array, int arrayIndex) => _collectionImplementation.CopyTo(array, arrayIndex);
        public IEnumerator<IReadOnlyObservableCollection<TGlyphData>> GetEnumerator() => _collectionImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collectionImplementation).GetEnumerator();
    }
}