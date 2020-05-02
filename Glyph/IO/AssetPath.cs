using System.Runtime.Serialization;

namespace Glyph.IO
{
    public struct AssetPath
    {
        public string Path { get; set; }

        public AssetPath(string path)
        {
            Path = path;
        }
        
        static public implicit operator string(AssetPath assetPath) => assetPath.Path;
        static public implicit operator AssetPath(string path) => new AssetPath(path);

        public override string ToString() => Path;
    }
}