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
        bool CanSetDisplayName { get; }
        void SetDisplayName(string newName);
        bool NotBinding { get; }
        new IGlyphComponent BindedObject { get; }
        IReadOnlyObservableCollection<IGlyphData> Children { get; }
        IReadOnlyObservableCollection<IGlyphDataChildrenSource> ChildrenSources { get; }
        IGlyphDataSource ParentSource { get; set; }
        IEnumerable<Type> SerializationKnownTypes { get; set; }
    }

    public interface IGlyphDataSource
    {
        IGlyphData Owner { get; }
        int Count { get; }
        int IndexOf(IGlyphData data);
        bool CanInsert(int index);
        bool CanInsert(int index, IGlyphData data);
        void Insert(int index, IGlyphData data);
        bool CanRemoveAt(int index);
        void RemoveAt(int index);
        bool CanMove(int oldIndex, int newIndex);
        void Move(int oldIndex, int newIndex);
    }

    public interface IGlyphDataPropertySource : IGlyphDataSource
    {
        string PropertyName { get; }
        bool HasData { get; }
    }

    public interface IGlyphDataChildrenSource : IGlyphDataPropertySource
    {
        IEnumerable<IGlyphData> Children { get; }
        INotifyCollectionChanged ChildrenNotifier { get; }
    }
}