using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Glyph.Composition.Messaging;
using Glyph.Composition.Utils;
using Glyph.Logging;
using Glyph.Messaging;
using Glyph.Observation.Properties;
using Microsoft.Extensions.Logging;
using Niddle.Attributes;
using Stave;
using Category = System.ComponentModel.CategoryAttribute;

namespace Glyph.Composition.Base
{

    public abstract class GlyphComponentBase : ConfigurableNotifyPropertyChanged, IGlyphComponent, ILogging
    {
        [Category(ComponentCategory.Activation)]
        public virtual bool Enabled { get; set; } = true;

        [Category(ComponentCategory.Activation)]
        public bool Active => Enabled && this.AllParents().All(x => x.Enabled);

        [Category(ComponentCategory.Activation)]
        public virtual bool Visible { get; set; } = true;

        [Category(ComponentCategory.Activation)]
        public bool Rendered => Visible && this.AllParents().All(x => x.Visible);

        [Category(ComponentCategory.Identification)]
        public Guid Id { get; }

        [Category(ComponentCategory.Identification)]
        public string Name { get; set; }

        [Category(ComponentCategory.Automation)]
        public ComponentRouterSystem Router { get; }

        [Category(ComponentCategory.Activation)]
        public bool IsDisposed { get; private set; }

        [Category(ComponentCategory.Composition)]
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

                    foreach (IGlyphComponent child in this.AllChildren())
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

                    foreach (IGlyphComponent child in this.AllChildren())
                    {
                        compositionMessage = MessageHelper.BuildGeneric(typeof(CompositionMessage<>), child.GetType(), t => t.GetConstructors().First(), child, child.Parent);
                        child.Router.Send(compositionMessage);
                    }
                }
            }
        }

        private readonly GlyphComponentContextInjection _contextInjection;

        private readonly CategoryLogger _categoryLogger;
        protected ILogger Logger => _categoryLogger;

        [Resolvable]
        ILogger ILogging.Logger
        {
            set => _categoryLogger.Logger = value;
        }

        public event EventHandler Disposed;

        protected GlyphComponentBase()
        {
            Type type = GetType();

            Id = Guid.NewGuid();
            Name = type.GetDisplayName();
            Router = new ComponentRouterSystem(this);

            _contextInjection = new GlyphComponentContextInjection(this);
            _categoryLogger = new CategoryLogger(type.GetDisplayName());
        }

        internal void SetupContextInjection()
        {
            _contextInjection.Setup();
        }

        public virtual void Initialize() { }
        public override string ToString() => $"{Name} ({Id.ToString().Substring(0, 4)})";

        public virtual void Store() { }
        public virtual void Restore() { }

        public override void Dispose()
        {
            _contextInjection.Dispose();

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

        public event Event<IComponentsChangedEventArgs<IGlyphComponent, IGlyphContainer>> HierarchyComponentsChanged
        {
            add => ComponentImplementation.HierarchyComponentsChanged += value;
            remove => ComponentImplementation.HierarchyComponentsChanged -= value;
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

        event Event<IComponentsChangedEventArgs> IComponent.HierarchyComponentsChanged
        {
            add => ComponentImplementationT0.HierarchyComponentsChanged += value;
            remove => ComponentImplementationT0.HierarchyComponentsChanged -= value;
        }

        event Event<IComponentsChangedEventArgs<IGlyphComponent>> IComponent<IGlyphComponent>.HierarchyComponentsChanged
        {
            add => ComponentImplementationT1.HierarchyComponentsChanged += value;
            remove => ComponentImplementationT1.HierarchyComponentsChanged -= value;
        }
    }
}