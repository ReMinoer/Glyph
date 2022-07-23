using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Diese;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Composition.Modelization.Base;
using Niddle;
using Simulacra.Binding;
using Simulacra.Binding.Array;
using Simulacra.Binding.Collection;
using Simulacra.Binding.Property;
using Simulacra.IO.Binding;
using Simulacra.Injection.Base;

namespace Glyph.Composition.Modelization
{
    public class BindedData<TData, T> : ResolvingDataBase<TData, T, IGlyphConfigurator<T>>, IGlyphCreator<T>, IGlyphConfigurator<T>, IHierarchicalData, IRestorable
        where TData : BindedData<TData, T>
        where T : class, IGlyphComponent
    {
        public class BindingCollections : IPropertyBindingsProvider<TData, T>, ICollectionBindingsProvider<TData, T>, IArrayBindingsProvider<TData, T>, IPathBindingsProvider<TData, T>
        {
            private readonly PropertyBindingCollection<TData, T> _propertyBindings = new PropertyBindingCollection<TData, T>();
            private readonly CollectionBindingCollection<TData, T> _collectionBindings = new CollectionBindingCollection<TData, T>();
            private readonly ArrayBindingCollection<TData, T> _arrayBindings = new ArrayBindingCollection<TData, T>();
            private readonly PathBindingCollection<TData, T> _pathBindings = new PathBindingCollection<TData, T>();

            PropertyBindingCollection<TData, T> IPropertyBindingsProvider<TData, T>.PropertyBindings => _propertyBindings;
            CollectionBindingCollection<TData, T> ICollectionBindingsProvider<TData, T>.CollectionBindings => _collectionBindings;
            ArrayBindingCollection<TData, T> IArrayBindingsProvider<TData, T>.ArrayBindings => _arrayBindings;
            PathBindingCollection<TData, T> IPathBindingsProvider<TData, T>.PathBindings => _pathBindings;

            internal void RegisterModules(BindingManager<T> bindingManager, TData owner)
            {
                bindingManager.Modules.Add(new PropertyBindingModule<TData, T>(owner, _propertyBindings));
                bindingManager.Modules.Add(new CollectionBindingModule<TData, T>(owner, _collectionBindings));
                bindingManager.Modules.Add(new ArrayBindingModule<TData, T>(owner, _arrayBindings));
                bindingManager.Modules.Add(new PathBindingModule<TData, T>(owner, _pathBindings));
            }
        }

        static public BindingCollections Bindings { get; } = new BindingCollections();

        [Browsable(false), IgnoreDataMember]
        public virtual string DisplayName => TypeName;
        static private readonly string TypeName = typeof(TData).GetDisplayName();

        [Browsable(false), IgnoreDataMember]
        new public T BindedObject => base.BindedObject;

        [Browsable(false), IgnoreDataMember]
        new public bool IsInstantiated => base.IsInstantiated;

        [Browsable(false), IgnoreDataMember]
        public SubDataCollection<IGlyphConfigurator<T>> SubConfigurators { get; }

        [Browsable(false), IgnoreDataMember]
        public SubDataCollection<IGlyphData> SubData { get; }

        [Browsable(false), IgnoreDataMember]
        public SubDataSourceCollection SubDataSources { get; }

        [Browsable(false), IgnoreDataMember]
        public IReadOnlyObservableCollection<IGlyphData> Children { get; }

        [Browsable(false), IgnoreDataMember]
        public IReadOnlyObservableCollection<IGlyphDataChildrenSource> ChildrenSources { get; }

        [Browsable(false), IgnoreDataMember]
        public IGlyphDataSource ParentSource { get; set; }

        protected override IDependencyResolver DependencyResolver
        {
            get => base.DependencyResolver;
            set
            {
                base.DependencyResolver = value;

                foreach (IGlyphConfigurator<T> subConfigurator in SubConfigurators)
                    subConfigurator.DependencyResolver = value;
            }
        }

        [Browsable(false), IgnoreDataMember]
        public IDependencyResolver Resolver
        {
            get => DependencyResolver;
            set => DependencyResolver = value;
        }

        [Browsable(false), IgnoreDataMember]
        public IEnumerable<Type> SerializationKnownTypes { get; set; }

        private bool _notBinding;
        bool IGlyphData.NotBinding => _notBinding;

        IGlyphComponent IGlyphData.BindedObject => BindedObject;
        protected override IEnumerable<IGlyphConfigurator<T>> SubConfiguratorsBase => SubConfigurators;

        private bool _isDisposed;
        bool INotifyDisposed.IsDisposed => _isDisposed;

        public event EventHandler Disposed;

        protected BindedData()
        {
            Bindings.RegisterModules(BindingManager, Owner);

            SubConfigurators = new SubDataCollection<IGlyphConfigurator<T>>(this);
            SubData = new SubDataCollection<IGlyphData>(this);
            SubDataSources = new SubDataSourceCollection(this);

            var readOnlySubConfigurators = new EnumerableReadOnlyObservableCollection<IGlyphData>(SubConfigurators);
            var readOnlySubData = new ReadOnlyObservableCollection<IGlyphData>(SubData);

            Children = new CompositeReadOnlyObservableCollection<IGlyphData>(readOnlySubData, readOnlySubConfigurators);
            ChildrenSources = new ReadOnlyObservableCollection<IGlyphDataChildrenSource>(SubDataSources);
        }

        public override string ToString() => DisplayName;

        public override T Create()
        {
            _notBinding = true;
            T obj = base.Create();
            _notBinding = false;
            return obj;
        }

        protected bool SetSubData<TSubData>(ref TSubData subData, TSubData value, Func<TSubData> getData, Action<TSubData> setData, [CallerMemberName] string propertyName = null)
            where TSubData : class, IGlyphData
        {
            if (EqualityComparer<TSubData>.Default.Equals(subData, value))
                return false;

            if (subData != null)
            {
                subData.ParentSource = null;
                SubData.Remove(subData);
            }

            subData = value;

            if (subData != null)
            {
                SubData.Add(subData);
                subData.ParentSource = new PropertySource<TSubData>(this, getData, setData, propertyName);
            }

            return true;
        }

        void IRestorable.Store() => Release();
        void IRestorable.Restore()
        {
            if (DependencyResolver is null)
                return;

            Instantiate();
        }

        protected override void DisposeBindedObject()
        {
            BindedObject?.Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();

            _isDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    public class PropertySource<TSubData> : IGlyphDataPropertySource
        where TSubData : class, IGlyphData
    {
        private readonly Func<TSubData> _getSubData;
        private readonly Action<TSubData> _setSubData;

        public IGlyphData Owner { get; }
        public string PropertyName { get; }

        public PropertySource(IGlyphData owner, Func<TSubData> getSubData, Action<TSubData> setSubData, string propertyName)
        {
            Owner = owner;
            _getSubData = getSubData;
            _setSubData = setSubData;
            PropertyName = propertyName;
        }

        public int IndexOf(IGlyphData data)
        {
            TSubData currentSubData = _getSubData();
            return currentSubData == data ? 0 : -1;
        }

        public void Set(int index, IGlyphData data)
        {
            if (index != 0)
                throw new ArgumentException("Index should be 0.");

            _setSubData((TSubData)data);
        }

        public void Unset(int index)
        {
            if (index != 0)
                throw new ArgumentException("Index should be 0.");

            _setSubData(null);
        }
    }
}