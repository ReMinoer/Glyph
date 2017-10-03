using System.ComponentModel;
using Diese;
using Stave;

namespace Glyph.Composition
{
    public abstract class GlyphContainer : GlyphContainer<IGlyphComponent>
    {
    }

    public abstract class GlyphContainer<TComponent> : Container<IGlyphComponent, IGlyphParent, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public string Name { get; set; }
        public bool IsFreeze { get; set; }
        public bool Disposed { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected GlyphContainer()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void Dispose()
        {
            foreach (TComponent component in Components)
                component.Dispose();

            InstanceManager.DisposeProcess(this);
            Disposed = true;
        }
    }
}