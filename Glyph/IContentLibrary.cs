using Glyph.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IContentLibrary
    {
        IAsset<T> GetAsset<T>(string assetPath);
        IAsset<T> GetLocalizedAsset<T>(string assetPath);
        IAsset<Effect> GetEffectAsset(string assetPath);
    }
}