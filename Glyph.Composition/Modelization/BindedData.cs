using System.ComponentModel;
using Glyph.Binding;

namespace Glyph.Composition.Modelization
{
    public class BindedData<TData, T> : BindedDataBase<T>
        where TData : BindedData<TData, T>
        where T : IGlyphComponent
    {
        public override string Name { get; set; }
        static protected OneSideBindingCollection<TData, T> Bindings { get; } = new OneSideBindingCollection<TData, T>();

        static BindedData()
        {
            Bindings.Add(x => x.Name, x => x.Name);
        }

        protected override void Configure(T obj)
        {
            base.Configure(obj);

            if (obj == null)
                return;

            var data = (TData)this;
            foreach (IOneWayBinding<TData, T> binding in Bindings)
                binding.UpdateView(data, obj);
        }

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (BindedObject == null)
                return;
            
            if (Bindings.TryGetBinding(propertyChangedEventArgs.PropertyName, out IOneWayBinding<TData, T> binding))
                binding.UpdateView((TData)this, BindedObject);
        }
    }
}