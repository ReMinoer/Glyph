using System;

namespace Glyph.Pipeline
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException(string assetPath)
            : base($"Asset {assetPath} not found")
        {
        }
    }
}