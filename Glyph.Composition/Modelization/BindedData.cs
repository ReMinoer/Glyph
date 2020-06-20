using System;
using System.Collections.Generic;
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
using Simulacra.Injection.Binding;

namespace Glyph.Composition.Modelization
{
    public class BindedData<TData, T> : ResolvingDataBase<TData, T, IGlyphConfigurator<T>>, IGlyphCreator<T>, IGlyphConfigurator<T>
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

        [GlyphCategory]
        public string Name { get; set; }

        [SimulacraCategory, IgnoreDataMember]
        new public T BindedObject => base.BindedObject;

        [SimulacraCategory, IgnoreDataMember]
        new public bool IsInstantiated => base.IsInstantiated;

        [SimulacraCategory]
        public SubDataCollection<IGlyphConfigurator<T>, TData, T> SubConfigurators { get; }
        
        [SimulacraCategory, IgnoreDataMember]
        public SubDataCollection<IGlyphData, TData, T> SubData { get; }

        [SimulacraCategory, IgnoreDataMember]
        public IReadOnlyObservableCollection<IGlyphData> Children { get; }

        [SimulacraCategory, IgnoreDataMember]
        public override IDependencyResolver DependencyResolver
        {
            protected get => base.DependencyResolver;
            set
            {
                base.DependencyResolver = value;

                foreach (IGlyphConfigurator<T> subConfigurator in SubConfigurators)
                    subConfigurator.DependencyResolver = value;
            }
        }
        
        [SimulacraCategory, IgnoreDataMember]
        public IDependencyResolver Resolver => DependencyResolver;

        [SimulacraCategory, IgnoreDataMember]
        public IEnumerable<Type> SerializationKnownTypes { get; set; }

        IGlyphComponent IGlyphData.BindedObject => BindedObject;
        protected override IEnumerable<IGlyphConfigurator<T>> SubConfiguratorsBase => SubConfigurators;

        static BindedData()
        {
            Bindings.From(x => x.Name).To(x => x.Name);
        }

        protected BindedData()
        {
            Bindings.RegisterModules(BindingManager, Owner);

            Name = typeof(T).GetDisplayName();

            SubConfigurators = new SubDataCollection<IGlyphConfigurator<T>, TData, T>(this);
            SubData = new SubDataCollection<IGlyphData, TData, T>(this);

            var readOnlySubConfiguratos = new EnumerableReadOnlyObservableCollection<IGlyphData>(SubConfigurators);
            var readOnlySubData = new ReadOnlyObservableCollection<IGlyphData>(SubData);
            Children = new CompositeReadOnlyObservableCollection<IGlyphData>(readOnlySubConfiguratos, readOnlySubData);
        }

        public override string ToString()
        {
            return Name ?? GetType().GetDisplayName();
        }

        protected override void DisposeBindedObject()
        {
            BindedObject?.Dispose();
        }
    }
}