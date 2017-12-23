using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Diese.Injection;
using Diese.Modelization;
using Glyph.Injection;

namespace Glyph.Modelization
{
    public abstract class BindedDataBase<T> : IGlyphCreator<T>, IBindedGlyphCreator<T>, IConfigurator<T>, INotifyPropertyChanged, IDisposable
    {
        [XmlIgnore]
        public IDependencyInjector Injector { protected get; set; }
        
        private T _bindedObject;
        [XmlIgnore]
        public virtual T BindedObject => _bindedObject;
        object IDataBindable.BindedObject => BindedObject;
        
        protected List<IBindedGlyphCreator> Children { get; } = new List<IBindedGlyphCreator>();
        IEnumerable<IInjectionClient> IGlyphCreator<T>.DataChildren => Children;
        IEnumerable<IBindedGlyphCreator> IBindedGlyphCreator.DataChildren => Children;

        public event PropertyChangedEventHandler PropertyChanged;

        protected BindedDataBase() => PropertyChanged += OnModelPropertyChanged;
        public void Dispose() => PropertyChanged -= OnModelPropertyChanged;

        protected virtual void OnModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Configure(_bindedObject);
        }

        private T Create()
        {
            T obj = New();
            Configure(obj);
            return obj;
        }

        protected virtual T New() => Injector.Resolve<T>();
        protected abstract void Configure(T obj);

        T ICreator<T>.Create() => Create();
        void IConfigurator<T>.Configure(T obj) => Configure(obj);
        void IBindedGlyphCreator.Instantiate() => _bindedObject = Create();
    }
}