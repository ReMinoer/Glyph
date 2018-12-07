using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese;
using Glyph.Composition.Messaging;
using Glyph.Messaging;
using Glyph.Observation.Properties;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphComponentBase : ConfigurableNotifyPropertyChanged, IGlyphComponent
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public ComponentRouterSystem Router { get; }
        public bool Disposed { get; private set; }

        public IGlyphContainer Parent
        {
            get => ComponentImplementation.Parent;
            set
            {
                if (value == Parent)
                    return;
                
                if (Parent != null)
                {
                    IMessage decompositionMessage = MessageHelper.BuildGeneric(typeof(DecompositionMessage<>), GetType(), t => t.GetConstructors().First(), this, Parent);
                    Router.Send(decompositionMessage);
                }
                
                ComponentImplementation.Parent = value;

                if (value != null)
                {
                    IMessage compositionMessage = MessageHelper.BuildGeneric(typeof(CompositionMessage<>), GetType(), t => t.GetConstructors().First(), this, value);
                    Router.Send(compositionMessage);
                }
            }
        }

        protected GlyphComponentBase()
        {
            Id = Guid.NewGuid();
            Name = GetType().GetDisplayName();
            Router = new ComponentRouterSystem(this);

            IMessage instantiatingMessage = MessageHelper.BuildGeneric(typeof(InstantiatingMessage<>), GetType(), t => t.GetConstructors().First(), this);
            Router.Global.Send(instantiatingMessage);
            Router.Local.Send(instantiatingMessage);
        }

        public virtual void Initialize() { }
        public override string ToString() => $"{Name} ({Id})";

        public override void Dispose()
        {
            IMessage disposingMessage = MessageHelper.BuildGeneric(typeof(DisposingMessage<>), GetType(), t => t.GetConstructors().First(), this);
            Router.Send(disposingMessage);

            Disposed = true;
            base.Dispose();
        }
        
        internal abstract IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation { get; }

        IEnumerable Stave.IComponent.Components => ComponentImplementation.Components;
        IEnumerable<IGlyphComponent> IComponent<IGlyphComponent>.Components => ComponentImplementation.Components;
        Stave.IComponent Stave.IComponent.Parent => ComponentImplementation.Parent;
        IGlyphComponent IComponent<IGlyphComponent>.Parent => ComponentImplementation.Parent;
    }
}