namespace Glyph.IO
{
    public struct FilePath
    {
        public string Path { get; set; }

        public FilePath(string path)
        {
            Path = PathUtils.Normalize(path);
        }

        static public FilePath None => new FilePath();
        public bool Defined => !string.IsNullOrWhiteSpace(Path);

        static public implicit operator string(FilePath filePath) => filePath.Path;
        static public implicit operator FilePath(string path) => new FilePath(path);

        public override string ToString() => Path;
    }
}