using System;

namespace Glyph.Observation.Properties
{
    public interface IPropertyChangedObservable
    {
        IObservable<string> ChangedProperties { get; }
    }
}