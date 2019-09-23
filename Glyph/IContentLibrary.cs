using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IContentLibrary
    {
        IServiceProvider ServiceProvider { get; }
        Task<T> GetOrLoad<T>(string assetName);
        Task<T> GetOrLoadLocalized<T>(string assetName);
        Task<Effect> GetOrLoadEffect(string assetName);
    }
}