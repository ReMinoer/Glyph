using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Composition.Base;
using Stave;
using Stave.Base;

namespace Glyph.Composition
{
    public class GlyphContainer : GlyphContainer<IGlyphComponent>
    {
    }

    public class GlyphContainer<TComponent> : GlyphContainerBase<TComponent>
        where TComponent : class, IGlyphComponent
    {
        protected GlyphContainer()
        {
            _component = new SealedOrderedComposite<IGlyphComponent, IGlyphContainer, TComponent>(this);
            HierarchyChanged += OnHierarchyChanged;
            HierarchyComponentAdded += OnHierarchyComponentAdded;

            ReadOnlyComponents = new ReadOnlyCollection<TComponent>(Components);
        }

        public override void Dispose()
        {
            HierarchyComponentAdded -= OnHierarchyComponentAdded;
            HierarchyChanged -= OnHierarchyChanged;

            foreach (TComponent component in ReadOnlyComponents)
                component.Dispose();

            base.Dispose();
        }

        private readonly SealedOrderedComposite<IGlyphComponent, IGlyphContainer, TComponent> _component;
        protected ComponentList<IGlyphComponent, IGlyphContainer, TComponent> Components => _component.Components;

        internal override IContainer<IGlyphComponent, IGlyphContainer, TComponent> ContainerImplementation => _component;
        internal override IEnumerable<TComponent> ReadOnlyComponents { get; }
    }
}