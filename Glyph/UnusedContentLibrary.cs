using System;
using System.Threading.Tasks;
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

        public Task<T> GetOrLoad<T>(string assetName)
        {
            throw new InvalidOperationException();
        }

        public Task<T> GetOrLoadLocalized<T>(string assetName)
        {
            throw new InvalidOperationException();
        }

        public Task<Effect> GetOrLoadEffect(string assetName)
        {
            throw new InvalidOperationException();
        }
    }
}