using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IContentLibrary
    {
        Task<T> GetOrLoad<T>(string assetPath);
        Task<T> GetOrLoadLocalized<T>(string assetPath);
        Task<Effect> GetOrLoadEffect(string assetPath);
    }
}