using System.Threading.Tasks;

namespace Glyph.Composition
{
    public interface ILoadContent : IGlyphComponent
    {
        Task LoadContent(IContentLibrary contentLibrary);
    }
}