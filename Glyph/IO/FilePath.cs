namespace Glyph.IO
{
    public readonly struct FilePath
    {
        public string Path { get; }

        public FilePath(string path)
        {
            Path = PathUtils.Normalize(path);
        }
        
        static public implicit operator string(FilePath filePath) => filePath.Path;
        static public implicit operator FilePath(string path) => new FilePath(path);

        public override string ToString() => Path;
    }
}