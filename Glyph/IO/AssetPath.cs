namespace Glyph.IO
{
    public struct AssetPath
    {
        public string Path { get; set; }

        public AssetPath(string path)
        {
            Path = path != null ? PathUtils.Normalize(path) : null;
        }

        static public FilePath None => new FilePath();
        public bool Defined => !string.IsNullOrWhiteSpace(Path);

        static public implicit operator string(AssetPath filePath) => filePath.Path;
        static public implicit operator AssetPath(string path) => new AssetPath(path);

        public override string ToString() => Path;
    }
}