using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Resolver;
using Stave;

namespace Glyph.Composition.Base
{
    public class GlyphComponentContextInjection : IDisposable
    {
        private readonly IGlyphComponent _component;

        private readonly Dictionary<object, List<GlyphResolvableInjectable>> _injected;
        private readonly List<GlyphResolvableInjectable> _injectables;

        public GlyphComponentContextInjection(IGlyphComponent component)
        {
            _component = component;

            _injected = new Dictionary<object, List<GlyphResolvableInjectable>>();
            _injectables = GlyphDependency.ResolvableMembersCache.ForType(component.GetType(), ResolveTargets.Parent | ResolveTargets.Fraternal).ToList();
        }

        public void Setup()
        {
            _component.HierarchyChanged += OnHierarchyChanged;
            _component.HierarchyComponentsChanged += OnHierarchyComponentChanged;
        }

        public void Dispose()
        {
            _component.HierarchyComponentsChanged -= OnHierarchyComponentChanged;
            _component.HierarchyChanged -= OnHierarchyChanged;
        }

        private void OnHierarchyChanged(object sender, IHierarchyChangedEventArgs<IGlyphComponent, IGlyphContainer> e)
        {
            switch (e.ChangeType)
            {
                case HierarchyChangeType.Link:
                    OnHierarchyLinked(e.Parent, e.Child);
                    break;
                case HierarchyChangeType.Unlink:
                    OnHierarchyUnlinked(e.Parent, e.Child);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void OnHierarchyLinked(IGlyphContainer parent, IGlyphComponent child)
        {
            if (_injectables.Count == 0)
                return;

            var injectedList = new List<(GlyphResolvableInjectable, IGlyphComponent)>();

            foreach (GlyphResolvableInjectable injectable in _injectables)
            {
                IGlyphComponent injectedComponent;

                if ((injectable.Targets & ResolveTargets.BrowseAllAncestors) != 0)
                {
                    foreach (IGlyphContainer childParent in child.AllParents())
                    {
                        if (InjectParentContext(injectable, childParent, out injectedComponent))
                        {
                            injectedList.Add((injectable, injectedComponent));
                            break;
                        }
                    }
                }
                else if (child == _component)
                {
                    if (InjectParentContext(injectable, parent, out injectedComponent))
                        injectedList.Add((injectable, injectedComponent));
                }
            }

            RegisterInjected(injectedList);
        }

        private bool InjectParentContext(GlyphResolvableInjectable injectable, IGlyphContainer parent, out IGlyphComponent injectedComponent)
        {
            injectedComponent = null;

            if ((injectable.Targets & ResolveTargets.Parent) != 0)
            {
                if (injectable.ResolvableInjectable.Type.IsInstanceOfType(parent))
                {
                    injectable.ResolvableInjectable.Inject(_component, parent);
                    injectedComponent = parent;
                    return true;
                }
            }

            if ((injectable.Targets & ResolveTargets.Fraternal) != 0)
            {
                foreach (IGlyphComponent parentComponent in parent.Components)
                {
                    if (parentComponent == _component)
                        continue;

                    if (injectable.ResolvableInjectable.Type.IsInstanceOfType(parentComponent))
                    {
                        injectable.ResolvableInjectable.Inject(_component, parentComponent);
                        injectedComponent = parentComponent;
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnHierarchyUnlinked(IGlyphContainer oldParent, IGlyphComponent keptParent)
        {
            if (_injected.Count == 0)
                return;

            foreach (IGlyphContainer parent in oldParent.AllParents())
            {
                if (_injected.ContainsKey(parent))
                    UnregisterInjected(parent);

                foreach (IGlyphComponent fraternal in parent.Components)
                    if (_injected.ContainsKey(fraternal))
                        UnregisterInjected(fraternal);
            }
        }

        private void OnHierarchyComponentChanged(object _, IComponentsChangedEventArgs<IGlyphComponent, IGlyphContainer> e)
        {
            switch (e.ChangeType)
            {
                case ComponentsChangeType.Add:
                    OnHierarchyComponentAdded(e.Parent, e.Component);
                    break;
                case ComponentsChangeType.Remove:
                    OnHierarchyComponentRemoved(e.Parent, e.Component);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void OnHierarchyComponentAdded(IGlyphContainer parent, IGlyphComponent component)
        {
            if (_injectables.Count == 0)
                return;

            var injectedList = new List<(GlyphResolvableInjectable, IGlyphComponent)>();

            foreach (GlyphResolvableInjectable injectable in _injectables)
            {
                if ((injectable.Targets & ResolveTargets.Fraternal) == 0)
                    continue;
                if ((injectable.Targets & ResolveTargets.BrowseAllAncestors) == 0 && parent != _component.Parent)
                    continue;
                if (component == _component)
                    continue;
                if (!injectable.ResolvableInjectable.Type.IsInstanceOfType(component))
                    continue;

                injectable.ResolvableInjectable.Inject(_component, component);
                injectedList.Add((injectable, component));
            }

            RegisterInjected(injectedList);
        }

        private void OnHierarchyComponentRemoved(IGlyphContainer parent, IGlyphComponent component)
        {
            if (_injected.Count == 0)
                return;

            if (_injected.ContainsKey(component))
                UnregisterInjected(component);
        }

        private void RegisterInjected(IEnumerable<(GlyphResolvableInjectable, IGlyphComponent)> injectedList)
        {
            foreach ((GlyphResolvableInjectable injectedInjectable, IGlyphComponent injectedComponent) in injectedList)
            {
                if (!_injected.TryGetValue(injectedComponent, out List<GlyphResolvableInjectable> injectedInjectables))
                    _injected[injectedComponent] = injectedInjectables = new List<GlyphResolvableInjectable>();
                
                injectedInjectables.Add(injectedInjectable);
                _injectables.Remove(injectedInjectable);
            }
        }

        private void UnregisterInjected(IGlyphComponent component)
        {
            if (!_injected.TryGetValue(component, out List<GlyphResolvableInjectable> injectedInjectables))
                return;

            _injected.Remove(component);
            foreach (GlyphResolvableInjectable injectable in injectedInjectables)
                _injectables.Add(injectable);
        }
    }
}