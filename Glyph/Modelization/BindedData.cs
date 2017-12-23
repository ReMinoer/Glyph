using System.ComponentModel;
using Glyph.Binding;

namespace Glyph.Modelization
{
    public class BindedData<TData, T> : BindedDataBase<T>
        where TData : BindedData<TData, T>
    {
        static protected OneSideBindingCollection<TData, T> Bindings { get; } = new OneSideBindingCollection<TData, T>();

        protected override void Configure(T obj)
        {
            if (obj == null)
                return;
            
            var data = (TData)this;
            foreach (IOneSideBinding<TData, T> binding in Bindings)
                binding.UpdateView(data, obj);
        }

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (BindedObject == null)
                return;
            
            if (Bindings.TryGetBinding(propertyChangedEventArgs.PropertyName, out IOneSideBinding<TData, T> binding))
                binding.UpdateView((TData)this, BindedObject);
        }
    }
}