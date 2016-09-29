using Diese;
using Stave;

namespace Glyph.Composition
{
    public class GlyphComponent : Component<IGlyphComponent, IGlyphParent>, IGlyphComponent
    {
        public string Name { get; set; }

        public GlyphComponent()
        {
            Name = GetType().GetDisplayName();
            InstanceManager.ConstructorProcess(this);
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            InstanceManager.DisposeProcess(this);
        }
    }
}