using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Glyph.Composition.Messaging;
using Glyph.Messaging;
using Glyph.Observation.Properties;
using Glyph.Resolver;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphComponentBase : ConfigurableNotifyPropertyChanged, IGlyphComponent
    {
        protected readonly List<GlyphResolvableInjectable> Injectables;

        public virtual bool Enabled { get; set; } = true;
        public bool Active => Enabled && this.ParentQueue().All(x => x.Enabled);

        public Guid Id { get; }
        public string Name { get; set; }
        public ComponentRouterSystem Router { get; }
        public bool IsDisposed { get; private set; }

        public event EventHandler Disposed;

        public IGlyphContainer Parent
        {
            get => ComponentImplementation.Parent;
            set
            {
                if (value == Parent)
                    return;
                
                if (Parent != null && Router.IsReady)
                {
                    IMessage decompositionMessage = MessageHelper.BuildGeneric(typeof(DecompositionMessage<>), GetType(), t => t.GetConstructors().First(), this, Parent);
                    Router.Send(decompositionMessage);

                    foreach (IGlyphComponent child in this.ChildrenQueue())
                    {
                        decompositionMessage = MessageHelper.BuildGeneric(typeof(DecompositionMessage<>), child.GetType(), t => t.GetConstructors().First(), child, child.Parent);
                        child.Router.Send(decompositionMessage);
                    }
                }
                
                ComponentImplementation.Parent = value;
                Router.Global = Parent?.Router.Global;

                if (value != null && Router.IsReady)
                {
                    IMessage compositionMessage = MessageHelper.BuildGeneric(typeof(CompositionMessage<>), GetType(), t => t.GetConstructors().First(), this, Parent);
                    Router.Send(compositionMessage);

                    foreach (IGlyphComponent child in this.ChildrenQueue())
                    {
                        compositionMessage = MessageHelper.BuildGeneric(typeof(CompositionMessage<>), child.GetType(), t => t.GetConstructors().First(), child, child.Parent);
                        child.Router.Send(compositionMessage);
                    }
                }
            }
        }

        protected GlyphComponentBase()
        {
            Type type = GetType();

            Id = Guid.NewGuid();
            Name = type.GetDisplayName();
            Router = new ComponentRouterSystem(this);

            Injectables = GlyphDependency.ResolvableMembersCache.ForType(type, ResolveTargets.Parent | ResolveTargets.Fraternal).ToList();
        }

        protected void OnHierarchyChanged(object sender, IHierarchyChangedEventArgs<IGlyphComponent, IGlyphContainer> e)
        {
            if (Injectables.Count == 0)
                return;

            if (e.LinkedParent == null)
                return;

            var injected = new List<GlyphResolvableInjectable>();

            foreach (GlyphResolvableInjectable injectable in Injectables)
            {
                if (e.LinkedChild == this && InjectParentContext(injectable, e.LinkedParent))
                {
                    injected.Add(injectable);
                    continue;
                }

                if ((injectable.Targets & ResolveTargets.BrowseAllAncestors) == 0)
                    continue;

                if (e.LinkedChild.ParentQueue().Any(parent => InjectParentContext(injectable, parent)))
                    injected.Add(injectable);
            }

            foreach (GlyphResolvableInjectable injectableToRemove in injected)
                Injectables.Remove(injectableToRemove);

            bool InjectParentContext(GlyphResolvableInjectable injectable, IGlyphContainer parent)
            {
                if ((injectable.Targets & ResolveTargets.Parent) != 0 && injectable.ResolvableInjectable.Type.IsInstanceOfType(parent))
                {
                    injectable.ResolvableInjectable.Inject(this, parent);
                    return true;
                }

                if ((injectable.Targets & ResolveTargets.Fraternal) == 0)
                    return false;

                foreach (IGlyphComponent parentComponent in parent.Components.Where(x => x != this && injectable.ResolvableInjectable.Type.IsInstanceOfType(x)))
                {
                    injectable.ResolvableInjectable.Inject(this, parentComponent);
                    return true;
                }

                return false;
            }
        }

        protected void OnHierarchyComponentAdded(object sender, IHierarchyComponentAddedEventArgs<IGlyphComponent, IGlyphContainer> e)
        {
            if (Injectables.Count == 0)
                return;

            var injected = new List<GlyphResolvableInjectable>();

            foreach (GlyphResolvableInjectable injectable in Injectables)
            {
                if ((injectable.Targets & ResolveTargets.Fraternal) == 0)
                    continue;
                if ((injectable.Targets & ResolveTargets.BrowseAllAncestors) == 0 && e.Parent != Parent)
                    continue;
                if (e.NewComponent == this)
                    continue;
                if (!injectable.ResolvableInjectable.Type.IsInstanceOfType(e.NewComponent))
                    continue;

                injectable.ResolvableInjectable.Inject(this, e.NewComponent);
                injected.Add(injectable);
            }

            foreach (GlyphResolvableInjectable injectableToRemove in injected)
                Injectables.Remove(injectableToRemove);
        }

        public virtual void Initialize() { }
        public override string ToString() => $"{Name} ({Id.ToString().Substring(0, 8)})";

        public override void Dispose()
        {
            HierarchyChanged -= OnHierarchyChanged;

            IMessage disposingMessage = MessageHelper.BuildGeneric(typeof(DisposingMessage<>), GetType(), t => t.GetConstructors().First(), this);
            Router.Send(disposingMessage);

            IsDisposed = true;
            Disposed?.Invoke(this, EventArgs.Empty);
            base.Dispose();
        }
        
        internal abstract IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation { get; }

        IEnumerable IComponent.Components => ComponentImplementation.Components;
        IEnumerable<IGlyphComponent> IComponent<IGlyphComponent>.Components => ComponentImplementation.Components;
        IComponent IComponent.Parent => ComponentImplementation.Parent;
        IGlyphComponent IComponent<IGlyphComponent>.Parent => ComponentImplementation.Parent;

        private IComponent ComponentImplementationT0 => ComponentImplementation;
        private IComponent<IGlyphComponent> ComponentImplementationT1 => ComponentImplementation;

        public event Event<IGlyphContainer> ParentChanged
        {
            add => ComponentImplementation.ParentChanged += value;
            remove => ComponentImplementation.ParentChanged -= value;
        }

        public event Event<IHierarchyChangedEventArgs<IGlyphComponent, IGlyphContainer>> HierarchyChanged
        {
            add => ComponentImplementation.HierarchyChanged += value;
            remove => ComponentImplementation.HierarchyChanged -= value;
        }

        public event Event<IHierarchyComponentAddedEventArgs<IGlyphComponent, IGlyphContainer>> HierarchyComponentAdded
        {
            add => ComponentImplementation.HierarchyComponentAdded += value;
            remove => ComponentImplementation.HierarchyComponentAdded -= value;
        }

        event Event<IComponent> IComponent.ParentChanged
        {
            add => ComponentImplementationT0.ParentChanged += value;
            remove => ComponentImplementationT0.ParentChanged -= value;
        }

        event Event<IGlyphComponent> IComponent<IGlyphComponent>.ParentChanged
        {
            add => ComponentImplementationT1.ParentChanged += value;
            remove => ComponentImplementationT1.ParentChanged -= value;
        }

        event Event<IHierarchyChangedEventArgs> IComponent.HierarchyChanged
        {
            add => ComponentImplementationT0.HierarchyChanged += value;
            remove => ComponentImplementationT0.HierarchyChanged -= value;
        }

        event Event<IHierarchyChangedEventArgs<IGlyphComponent>> IComponent<IGlyphComponent>.HierarchyChanged
        {
            add => ComponentImplementationT1.HierarchyChanged += value;
            remove => ComponentImplementationT1.HierarchyChanged -= value;
        }

        event Event<IHierarchyComponentAddedEventArgs> IComponent.HierarchyComponentAdded
        {
            add => ComponentImplementationT0.HierarchyComponentAdded += value;
            remove => ComponentImplementationT0.HierarchyComponentAdded -= value;
        }

        event Event<IHierarchyComponentAddedEventArgs<IGlyphComponent>> IComponent<IGlyphComponent>.HierarchyComponentAdded
        {
            add => ComponentImplementationT1.HierarchyComponentAdded += value;
            remove => ComponentImplementationT1.HierarchyComponentAdded -= value;
        }
    }
}