using System.Collections;
using System.Collections.Generic;
using Diese;
using Glyph.PropertyChanged;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphComponentBase : ConfigurableNotifyPropertyChanged, IGlyphComponent
    {
        public string Name { get; set; }
        public bool Disposed { get; private set; }

        protected GlyphComponentBase()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize() { }
        public override string ToString() => Name;

        public override void Dispose()
        {
            InstanceManager.DisposeProcess(this);
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