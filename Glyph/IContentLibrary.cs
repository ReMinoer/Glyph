using System;
using System.Threading.Tasks;
using Glyph.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IContentLibrary
    {
        IServiceProvider ServiceProvider { get; }
        Task<T> GetOrLoad<T>(string assetPath);
        Task<T> GetOrLoadLocalized<T>(string assetPath);
        Task<Effect> GetOrLoadEffect(string assetPath);
    }
}