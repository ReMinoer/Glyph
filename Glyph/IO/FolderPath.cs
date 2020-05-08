namespace Glyph.IO
{
    public readonly struct FolderPath
    {
        public string Path { get; }

        public FolderPath(string path)
        {
            Path = PathUtils.NormalizeFolder(path);
        }

        static public implicit operator string(FolderPath folderPath) => folderPath.Path;
        static public implicit operator FolderPath(string path) => new FolderPath(path);

        public override string ToString() => Path;
    }
}