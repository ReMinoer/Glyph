using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Diese;
using Diese.Collections;
using Niddle;
using Simulacra;

namespace Glyph.Composition.Modelization
{
    public abstract class BindedDataBase<T> : IGlyphCreator<T>, INotifyPropertyChanged
        where T : IGlyphComponent
    {
        private IDependencyResolver _resolver;

        [Category("Creator"), IgnoreDataMember]
        public IReadOnlyObservableCollection<IGlyphCreator> Children { get; }

        protected ObservableList<IGlyphConfigurator<T>> SubConfigurators { get; }
        IEnumerable<IConfigurator<T>> ICompositeConfigurator<T>.SubConfigurators => SubConfigurators;
        IEnumerable<IGlyphConfigurator<T>> ICompositeConfigurator<T, IGlyphConfigurator<T>>.SubConfigurators => SubConfigurators;

        [Category("Component")]
        public abstract string Name { get; set; }

        [Category("Creator"), IgnoreDataMember]
        public bool IsInstantiated { get; private set; }

        [Category("Creator"), IgnoreDataMember]
        public T BindedObject { get; private set; }
        IGlyphComponent IDataBindable<IGlyphComponent>.BindedObject => BindedObject;
        object IDataBindable.BindedObject => BindedObject;

        [IgnoreDataMember]
        public IDependencyResolver Resolver
        {
            protected get { return _resolver; }
            set
            {
                _resolver = value;

                foreach (IGlyphConfigurator<T> configurator in SubConfigurators)
                    configurator.Resolver = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected BindedDataBase()
        {
            SubConfigurators = new ObservableList<IGlyphConfigurator<T>>();
            Children = new CompositeReadOnlyObservableCollection<IGlyphCreator>(SubConfigurators.Select(x => x.Children));
            PropertyChanged += OnModelPropertyChanged;
        }

        public void Dispose()
        {
            PropertyChanged -= OnModelPropertyChanged;
            (BindedObject as IDisposable)?.Dispose();
            BindedObject = default(T);
        }

        protected virtual void OnModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Configure(BindedObject);
        }

        public void Instantiate()
        {
            if (BindedObject != null)
                throw new InvalidOperationException();

            BindedObject = New();

            if (Name == null)
                Name = BindedObject.Name;

            Configure(BindedObject);
            IsInstantiated = true;
        }

        private T Create()
        {
            T obj = New();
            Configure(obj);
            return obj;
        }

        protected virtual T New() => Resolver.Resolve<T>();

        protected virtual void Configure(T obj)
        {
            if (obj == null)
                return;

            foreach (IGlyphConfigurator<T> subConfigurator in SubConfigurators)
                subConfigurator.Configure(obj);
        }

        T ICreator<T>.Create() => Create();
        void IConfigurator<T>.Configure(T obj) => Configure(obj);

        public override string ToString()
        {
            return Name ?? GetType().GetDisplayName();
        }
    }
}