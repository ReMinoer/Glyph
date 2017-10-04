using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Diese;
using Stave;

namespace Glyph.Composition.Base
{
    public abstract class GlyphComponentBase : IGlyphComponent
    {
        public string Name { get; set; }
        public bool Disposed { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected GlyphComponentBase()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize() { }
        public override string ToString() => Name;

        public virtual void Dispose()
        {
            InstanceManager.DisposeProcess(this);
            Disposed = true;
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