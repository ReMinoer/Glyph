using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Composition.Base;
using Glyph.Composition.Utils;
using Stave;
using Category = System.ComponentModel.CategoryAttribute;

namespace Glyph.Composition
{
    public class GlyphComposite : GlyphComposite<IGlyphComponent>
    {
    }

    public class GlyphComposite<TComponent> : GlyphContainerBase<TComponent>, IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        private readonly Dictionary<object, TComponent> _keyedComponents;
        private readonly Dictionary<object, SubGlyphComposite<TComponent>> _keyedSubComposite;

        protected GlyphComposite()
        {
            _component = new OrderedComposite<IGlyphComponent, IGlyphContainer, TComponent>(this);

            _keyedComponents = new Dictionary<object, TComponent>();
            _keyedSubComposite = new Dictionary<object, SubGlyphComposite<TComponent>>();

            SetupComponent();
        }

        private readonly IOrderedComposite<IGlyphComponent, IGlyphContainer, TComponent> _component;

        internal override IContainer<IGlyphComponent, IGlyphContainer, TComponent> ContainerImplementation => _component;
        internal override IEnumerable<TComponent> ReadOnlyComponents => _component.Components;
        
        [Category(ComponentCategory.Composition)]
        public IWrappedObservableList<TComponent> Components => _component.Components;
        IWrappedObservableCollection<TComponent> IComposite<IGlyphComponent, IGlyphContainer, TComponent>.Components => _component.Components;

        public virtual TComponent this[int index]
        {
            get => _component[index];
            set => _component[index] = value;
        }

        public virtual void Add(TComponent item) => _component.Add(item);
        public virtual bool Remove(TComponent item)
        {
            _keyedComponents.Remove(x => x.Value == item);

            if (!_component.Remove(item))
                return false;

            foreach (SubGlyphComposite<TComponent> subGlyphComposite in _keyedSubComposite.Values)
                subGlyphComposite.OnRemove(item);

            return true;
        }

        public virtual void Clear()
        {
            _keyedComponents.Clear();
            _keyedSubComposite.Clear();
            _component.Clear();
        }

        public void RemoveAndDispose(TComponent item)
        {
            Remove(item);
            item.Dispose();
        }

        public void ClearAndDisposeComponents()
        {
            TComponent[] components = Components.ToArray();
            Clear();

            foreach (TComponent component in components)
                component.Dispose();
        }

        public bool Contains(TComponent item) => _component.Contains(item);

        public int IndexOf(TComponent item) => _component.IndexOf(item);
        public virtual void Insert(int index, TComponent item) => _component.Insert(index, item);
        public virtual void RemoveAt(int index) => _component.RemoveAt(index);

        protected bool SetPropertyComponent<T>(ref T component, T value, int insertIndex = -1, bool disposeOnRemove = false)
            where T : class, TComponent
        {
            if (component == value)
                return false;

            if (component != null)
            {
                Remove(component);
                if (disposeOnRemove)
                    component.Dispose();
            }

            component = value;

            if (component != null)
            {
                if (insertIndex != -1 && insertIndex < Components.Count)
                    Insert(insertIndex, component);
                else
                    Add(component);
            }

            return true;
        }
        
        public T GetKeyedComponent<T>(object key)
            where T : class, TComponent
        {
            return _keyedComponents.TryGetValue(key, out TComponent component) ? (T)component : null;
        }

        public bool SetKeyedComponent(object key, TComponent component)
        {
            TComponent currentComponent = GetKeyedComponent<TComponent>(key);
            if (component == currentComponent)
                return false;

            if (currentComponent != null)
            {
                RemoveAndDispose(currentComponent);
                _keyedComponents.Remove(key);
            }

            if (component != null)
            {
                if (!Contains(component))
                    Add(component);
                _keyedComponents[key] = component;
            }

            return true;
        }

        public IList<TComponent> CreateSubComposite(object key)
        {
            var subComposite = new SubGlyphComposite<TComponent>(this);
            _keyedSubComposite.Add(key, subComposite);
            return subComposite;
        }

        public bool RemoveSubComposite(object key) => _keyedSubComposite.Remove(key);
        public IList<TComponent> GetSubComposite(object key) => _keyedSubComposite[key];
    }
}