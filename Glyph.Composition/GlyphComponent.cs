using Diese.Composition;

namespace Glyph.Composition
{
    public class GlyphComponent : Component<IGlyphComponent, IGlyphParent>, IGlyphComponent
    {
        public GlyphComponent()
        {
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