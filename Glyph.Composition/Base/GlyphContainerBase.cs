using System.Collections.Generic;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphContainerBase<TComponent> : GlyphComponentBase, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public bool IsFreeze { get; set; }

        internal abstract IContainer<IGlyphComponent, IGlyphContainer, TComponent> ContainerImplementation { get; }
        internal override sealed IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation => ContainerImplementation;

        internal abstract IEnumerable<TComponent> ReadOnlyComponents { get; }
        IEnumerable<TComponent> IContainer<IGlyphComponent, IGlyphContainer, TComponent>.Components => ReadOnlyComponents;

        bool IContainer.Opened => ContainerImplementation.Opened;
        void IContainer<IGlyphComponent>.Link(IGlyphComponent child) => ContainerImplementation.Link(child);
        void IContainer<IGlyphComponent>.Unlink(IGlyphComponent child) => ContainerImplementation.Unlink(child);
        bool IContainer<IGlyphComponent>.TryLink(IGlyphComponent child) => ContainerImplementation.TryLink(child);
        bool IContainer<IGlyphComponent>.TryUnlink(IGlyphComponent child) => ContainerImplementation.TryUnlink(child);
        void IContainer<IGlyphComponent, IGlyphContainer, TComponent>.Link(TComponent child) => ContainerImplementation.Link(child);
        void IContainer<IGlyphComponent, IGlyphContainer, TComponent>.Unlink(TComponent child) => ContainerImplementation.Unlink(child);
        bool IContainer<IGlyphComponent, IGlyphContainer, TComponent>.TryLink(TComponent child) => ContainerImplementation.TryLink(child);
        bool IContainer<IGlyphComponent, IGlyphContainer, TComponent>.TryUnlink(TComponent child) => ContainerImplementation.TryUnlink(child);

        private IContainer ContainerImplementationT0 => ContainerImplementation;
        private IContainer<IGlyphComponent> ContainerImplementationT1 => ContainerImplementation;

        public event Event<TComponent> ComponentAdded
        {
            add => ContainerImplementation.ComponentAdded += value;
            remove => ContainerImplementation.ComponentAdded -= value;
        }

        event Event<IComponent> IContainer.ComponentAdded
        {
            add => ContainerImplementationT0.ComponentAdded += value;
            remove => ContainerImplementationT0.ComponentAdded -= value;
        }

        event Event<IGlyphComponent> IContainer<IGlyphComponent>.ComponentAdded
        {
            add => ContainerImplementationT1.ComponentAdded += value;
            remove => ContainerImplementationT1.ComponentAdded -= value;
        }
    }
}