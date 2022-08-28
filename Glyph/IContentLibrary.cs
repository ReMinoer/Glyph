using Glyph.Content;

namespace Glyph
{
    public interface IContentLibrary
    {
        string WorkingDirectory { get; }
        IAsset<T> GetAsset<T>(string assetPath);
        IAsset<T> GetLocalizedAsset<T>(string assetPath);
    }
}