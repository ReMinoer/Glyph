using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;
using Diese.Collections;
using Diese.Injection;
using Diese.Modelization;

namespace Glyph.Composition.Modelization
{
    public class FactoryConfigurator<TData, TParent> : ObservableCollection<TData>, IGlyphConfigurator<TParent>
        where TData : IGlyphCreator
        where TParent : IGlyphComposite<IGlyphComponent>
    {
        private bool _itemsInitialized;
        private IDependencyInjector _injector;
        private readonly IGlyphCreator<TParent> _parent;

        [XmlIgnore]
        public IDependencyInjector Injector
        {
            protected get { return _injector; }
            set
            {
                if (_injector == null && value != null)
                    CollectionChanged += OnCollectionChanged;
                else if (_injector != null && value == null)
                    CollectionChanged -= OnCollectionChanged;

                _injector = value;
                
                foreach (TData newData in this)
                {
                    newData.Injector = Injector;

                    if (_parent.IsInstantiated)
                    {
                        newData.Instantiate();
                        BindedObject.Add(newData.BindedObject);
                    }
                }
            }
        }

        private readonly IReadOnlyObservableCollection<TData> _children;
        IReadOnlyObservableCollection<IGlyphCreator> IGlyphConfigurator.Children => (IReadOnlyObservableCollection<IGlyphCreator>)_children;
        IEnumerable<IConfigurator<TParent>> ICompositeConfigurator<TParent>.SubConfigurators => Enumerable.Empty<IGlyphConfigurator<TParent>>();
        IEnumerable<IGlyphConfigurator<TParent>> ICompositeConfigurator<TParent, IGlyphConfigurator<TParent>>.SubConfigurators => Enumerable.Empty<IGlyphConfigurator<TParent>>();

        [XmlIgnore]
        public TParent BindedObject => (TParent)_parent.BindedObject;
        IGlyphComponent IDataBindable<IGlyphComponent>.BindedObject => BindedObject;
        object IDataBindable.BindedObject => BindedObject;

        public override sealed event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => base.CollectionChanged += value;
            remove => base.CollectionChanged -= value;
        }

        public FactoryConfigurator(IGlyphCreator<TParent> parent)
        {
            _parent = parent;
            _children = new Diese.Collections.ReadOnlyObservableCollection<TData>(this);
        }

        public void Configure(TParent obj)
        {
            foreach (TData data in this)
            {
                if (data.BindedObject != null)
                    obj.Remove(data.BindedObject);

                if (!data.IsInstantiated)
                    data.Instantiate();

                obj.Add(data.BindedObject);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (TData data in this)
                {
                    BindedObject.Remove(data.BindedObject);
                    data.Dispose();
                }
                return;
            }

            if (e.OldItems != null)
                foreach (TData oldData in e.OldItems)
                {
                    BindedObject.Remove(oldData.BindedObject);
                    oldData.Dispose();
                }

            if (e.NewItems != null)
                foreach (TData newData in e.NewItems)
                {
                    newData.Injector = Injector;
                    newData.Instantiate();
                    BindedObject.Add(newData.BindedObject);
                }
        }
    }
}