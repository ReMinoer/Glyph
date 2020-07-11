using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class UnusedContentLibrary : IContentLibrary
    {
        public UnusedContentLibrary()
        {
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