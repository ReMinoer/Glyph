using System.Collections.Generic;
using Glyph.Composition.Utils;
using Stave;
using Category = System.ComponentModel.CategoryAttribute;

namespace Glyph.Composition.Base
{
    public abstract class GlyphContainerBase<TComponent> : GlyphComponentBase, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        [Category(ComponentCategory.Activation)]
        public bool IsFreeze { get; set; }

        internal abstract IContainer<IGlyphComponent, IGlyphContainer, TComponent> ContainerImplementation { get; }
        internal override sealed IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation => ContainerImplementation;

        internal abstract IEnumerable<TComponent> ReadOnlyComponents { get; }
        IEnumerable<TComponent> IContainer<IGlyphComponent, IGlyphContainer, TComponent>.Components => ReadOnlyComponents;
        
        public override void Dispose()
        {
            foreach (TComponent component in ReadOnlyComponents)
                component.Dispose();

            base.Dispose();
        }

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
        private IContainer<IGlyphComponent, IGlyphContainer> ContainerImplementationT2 => ContainerImplementation;

        public event Event<IComponentsChangedEventArgs<IGlyphComponent, IGlyphContainer, TComponent>> ComponentsChanged
        {
            add => ContainerImplementation.ComponentsChanged += value;
            remove => ContainerImplementation.ComponentsChanged -= value;
        }

        event Event<IComponentsChangedEventArgs> IContainer.ComponentsChanged
        {
            add => ContainerImplementationT0.ComponentsChanged += value;
            remove => ContainerImplementationT0.ComponentsChanged -= value;
        }

        event Event<IComponentsChangedEventArgs<IGlyphComponent>> IContainer<IGlyphComponent>.ComponentsChanged
        {
            add => ContainerImplementationT1.ComponentsChanged += value;
            remove => ContainerImplementationT1.ComponentsChanged -= value;
        }

        event Event<IComponentsChangedEventArgs<IGlyphComponent, IGlyphContainer>> IContainer<IGlyphComponent, IGlyphContainer>.ComponentsChanged
        {
            add => ContainerImplementationT2.ComponentsChanged += value;
            remove => ContainerImplementationT2.ComponentsChanged -= value;
        }
    }
}