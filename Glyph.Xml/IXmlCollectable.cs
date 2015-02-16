using System.Xml.Serialization;

namespace Glyph.Xml
{
    public interface IXmlCollectable : IXmlSerializable
    {
        XmlCollection GenerateXmlCollection();
        void Initialize(XmlCollection c);
    }
}