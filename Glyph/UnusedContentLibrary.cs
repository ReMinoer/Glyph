using System;
using System.Threading.Tasks;
using Glyph.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class UnusedContentLibrary : IContentLibrary
    {
        public IServiceProvider ServiceProvider { get; }

        public UnusedContentLibrary(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Task<T> GetOrLoad<T>(string assetPath)
        {
            throw new InvalidOperationException();
        }

        public Task<T> GetOrLoadLocalized<T>(string assetPath)
        {
            throw new InvalidOperationException();
        }

        public Task<Effect> GetOrLoadEffect(string assetPath)
        {
            throw new InvalidOperationException();
        }
    }
}