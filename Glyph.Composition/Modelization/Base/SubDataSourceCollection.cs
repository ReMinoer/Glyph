using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Diese.Collections.Observables;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization.Base
{
    public class SubDataSourceCollection : IObservableCollection<IGlyphDataChildrenSource>
    {
        private readonly IObservableCollection<IGlyphDataChildrenSource> _collectionImplementation;
        private readonly IDependencyResolverClient _parentData;
        private readonly Dictionary<INotifyCollectionChanged, IGlyphDataChildrenSource> _sourceByNotifier;

        public int Count => _collectionImplementation.Count;
        bool ICollection<IGlyphDataChildrenSource>.IsReadOnly => _collectionImplementation.IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SubDataSourceCollection(IDependencyResolverClient parentData)
        {
            _collectionImplementation = new ObservableList<IGlyphDataChildrenSource>();
            _parentData = parentData;
            _sourceByNotifier = new Dictionary<INotifyCollectionChanged, IGlyphDataChildrenSource>();

            _collectionImplementation.PropertyChanged += OnPropertyChanged;
            _collectionImplementation.CollectionChanged += OnCollectionChanged;
        }

        public void Add(IGlyphDataChildrenSource item)
        {
            _collectionImplementation.Add(item);
            RegisterSource(item);
        }

        public bool Remove(IGlyphDataChildrenSource item)
        {
            UnregisterSource(item);
            return _collectionImplementation.Remove(item);
        }

        public void Clear()
        {
            foreach (IGlyphDataChildrenSource childrenSource in _collectionImplementation)
                UnregisterSource(childrenSource);

            _collectionImplementation.Clear();
        }

        public bool Contains(IGlyphDataChildrenSource item) => _collectionImplementation.Contains(item);
        public void CopyTo(IGlyphDataChildrenSource[] array, int arrayIndex) => throw new NotSupportedException();
        public IEnumerator<IGlyphDataChildrenSource> GetEnumerator() => _collectionImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_collectionImplementation).GetEnumerator();

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(sender, e);

        private void RegisterSource(IGlyphDataChildrenSource childrenSource)
        {
            if (childrenSource is null)
                return;
            
            foreach (IGlyphData child in childrenSource.Children)
                RegisterChild(child, childrenSource);

            _sourceByNotifier[childrenSource.ChildrenNotifier] = childrenSource;
            childrenSource.ChildrenNotifier.CollectionChanged += OnChildrenCollectionChanged;
        }

        private void UnregisterSource(IGlyphDataChildrenSource childrenSource)
        {
            if (childrenSource is null)
                return;

            childrenSource.ChildrenNotifier.CollectionChanged -= OnChildrenCollectionChanged;
            _sourceByNotifier.Remove(childrenSource.ChildrenNotifier);

            foreach (IGlyphData child in childrenSource.Children)
                UnregisterChild(child);
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IGlyphDataChildrenSource childrenSource = _sourceByNotifier[(INotifyCollectionChanged)sender];

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    RegisterChildren(e.NewItems, childrenSource);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    UnregisterChildren(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UnregisterChildren(e.OldItems);
                    RegisterChildren(e.NewItems, childrenSource);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UnregisterChildren(childrenSource.Children);
                    break;
            }
        }

        private void RegisterChildren(IEnumerable children, IGlyphDataChildrenSource childrenSource)
        {
            foreach (IGlyphData child in children)
                RegisterChild(child, childrenSource);
        }

        private void UnregisterChildren(IEnumerable children)
        {
            foreach (IGlyphData child in children)
                UnregisterChild(child);
        }

        private void RegisterChild(IGlyphData child, IGlyphDataChildrenSource childrenSource)
        {
            child.ParentSource = childrenSource;
        }

        private void UnregisterChild(IGlyphData child)
        {
            child.ParentSource = null;
        }
    }
}