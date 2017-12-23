using System.Collections;
using System.Collections.Generic;
using Diese;
using Glyph.Composition.Messaging;
using Glyph.Messaging;
using Glyph.Observation.Properties;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphComponentBase : ConfigurableNotifyPropertyChanged, IGlyphComponent
    {
        static public Router MainRouter { get; } = new Router();
        
        public string Name { get; set; }
        public bool Disposed { get; private set; }

        protected GlyphComponentBase()
        {
            Name = GetType().GetDisplayName();
            MainRouter.Send(MessageHelper.BuildGeneric(typeof(InstantiatingMessage<>), this));
        }

        public virtual void Initialize() { }
        public override string ToString() => Name;

        public override void Dispose()
        {
            MainRouter.Send(MessageHelper.BuildGeneric(typeof(DisposingMessage<>), this));
            Disposed = true;
            base.Dispose();
        }

        internal abstract IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation { get; }

        IEnumerable Stave.IComponent.Components => ComponentImplementation.Components;
        IEnumerable<IGlyphComponent> IComponent<IGlyphComponent>.Components => ComponentImplementation.Components;
        Stave.IComponent Stave.IComponent.Parent => ComponentImplementation.Parent;
        IGlyphComponent IComponent<IGlyphComponent>.Parent => ComponentImplementation.Parent;

        public IGlyphContainer Parent
        {
            get => ComponentImplementation.Parent;
            set => ComponentImplementation.Parent = value;
        }
    }
}