namespace Glyph.IO
{
    public struct FolderPath
    {
        public string Path { get; set; }

        public FolderPath(string path)
        {
            Path = path != null ? PathUtils.Normalize(path) : null;
        }

        static public FolderPath None => new FolderPath();
        public bool Defined => !string.IsNullOrWhiteSpace(Path);

        static public implicit operator string(FolderPath folderPath) => folderPath.Path;
        static public implicit operator FolderPath(string path) => new FolderPath(path);

        public override string ToString() => Path;
    }
}