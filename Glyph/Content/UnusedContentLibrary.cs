using System;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Content
{
    public class UnusedContentLibrary : IContentLibrary
    {
        public string WorkingDirectory => null;
        public IAsset<T> GetAsset<T>(string assetPath) => throw new InvalidOperationException();
        public IAsset<T> GetLocalizedAsset<T>(string assetPath) => throw new InvalidOperationException();
        public IAsset<Effect> GetEffectAsset(string assetPath) => throw new InvalidOperationException();
    }
}