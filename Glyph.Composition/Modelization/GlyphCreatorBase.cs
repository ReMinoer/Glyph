using System.Xml.Serialization;
using Diese.Injection;

namespace Glyph.Composition.Modelization
{
    public abstract class GlyphCreatorBase<T> : IGlyphCreator<T>
    {
        [XmlIgnore]
        public IDependencyInjector Injector { protected get; set; }
        public abstract T Create();
    }
}