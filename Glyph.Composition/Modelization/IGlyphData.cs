using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Diese.Collections.Observables.ReadOnly;
using Simulacra;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphData : IBindableData, IDependencyResolverClient, INotifyDisposed
    {
        string DisplayName { get; }
        bool NotBinding { get; }
        new IGlyphComponent BindedObject { get; }
        IReadOnlyObservableCollection<IGlyphData> Children { get; }
        IReadOnlyObservableCollection<IGlyphDataChildrenSource> ChildrenSources { get; }
        IGlyphDataSource ParentSource { get; set; }
        IEnumerable<Type> SerializationKnownTypes { get; set; }
    }

    public interface IGlyphDataSource
    {
        int IndexOf(IGlyphData data);
        void Set(int index, IGlyphData data);
        void Unset(int index);
    }

    public interface IGlyphDataPropertySource : IGlyphDataSource
    {
        string PropertyName { get; }
    }

    public interface IGlyphDataChildrenSource : IGlyphDataPropertySource
    {
        IEnumerable<IGlyphData> Children { get; }
        INotifyCollectionChanged ChildrenNotifier { get; }
    }
}