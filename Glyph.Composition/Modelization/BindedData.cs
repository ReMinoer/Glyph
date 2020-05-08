using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Diese;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Composition.Modelization.Base;
using Niddle;
using Simulacra.Binding;
using Simulacra.IO.Binding;
using Simulacra.Injection.Base;
using Simulacra.Injection.Binding;

namespace Glyph.Composition.Modelization
{
    public class BindedData<TData, T> : ResolvingDataBase<TData, T, IGlyphConfigurator<T>>, IGlyphCreator<T>, IGlyphConfigurator<T>
        where TData : BindedData<TData, T>
        where T : class, IGlyphComponent
    {
        static public PathBindingCollection<TData, T> PathBindings { get; }

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
            PathBindings = new PathBindingCollection<TData, T>();
            PropertyBindings.From(x => x.Name).To(x => x.Name);
        }

        protected BindedData()
        {
            BindingManager.Modules.Add(new PathBindingModule<TData, T>(Owner, PathBindings));

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