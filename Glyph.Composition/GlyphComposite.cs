using System.Collections.Generic;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Composition.Base;
using Stave;

namespace Glyph.Composition
{
    public class GlyphComposite : GlyphComposite<IGlyphComponent>
    {
    }

    public class GlyphComposite<TComponent> : GlyphContainerBase<TComponent>, IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        protected GlyphComposite()
        {
            _component = new Composite<IGlyphComponent, IGlyphContainer, TComponent>(this);
            HierarchyChanged += OnHierarchyChanged;
            HierarchyComponentAdded += OnHierarchyComponentAdded;
        }

        public override void Dispose()
        {
            HierarchyComponentAdded -= OnHierarchyComponentAdded;
            HierarchyChanged -= OnHierarchyChanged;

            foreach (TComponent component in ReadOnlyComponents)
                component.Dispose();

            Clear();

            base.Dispose();
        }

        private readonly IComposite<IGlyphComponent, IGlyphContainer, TComponent> _component;

        internal override IContainer<IGlyphComponent, IGlyphContainer, TComponent> ContainerImplementation => _component;
        internal override IEnumerable<TComponent> ReadOnlyComponents => _component.Components;

        public IWrappedObservableCollection<TComponent> Components => _component.Components;
        
        public virtual void Add(TComponent item) => _component.Add(item);
        public virtual bool Remove(TComponent item) => _component.Remove(item);
        public virtual void Clear() => _component.Clear();
        public bool Contains(TComponent item) => _component.Contains(item);
    }
}