using System.Threading.Tasks;

namespace Glyph.Composition
{
    public interface ILoadContentTask
    {
        Task LoadContent(IContentLibrary contentLibrary);
    }

    public interface ILoadContent : IGlyphComponent, ILoadContentTask
    {
    }
}