using System;

namespace Glyph.PropertyChanged
{
    public interface IPropertyChangedObservable
    {
        IObservable<string> ChangedProperties { get; }
    }
}