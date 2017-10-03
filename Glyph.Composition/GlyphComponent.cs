using System.ComponentModel;
using System.Runtime.CompilerServices;
using Diese;
using Stave;

namespace Glyph.Composition
{
    public class GlyphComponent : Component<IGlyphComponent, IGlyphParent>, IGlyphComponent
    {
        public string Name { get; set; }
        public bool Disposed { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public GlyphComponent()
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
            InstanceManager.DisposeProcess(this);
            Disposed = true;
        }
    }
}