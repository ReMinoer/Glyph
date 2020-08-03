namespace Glyph.IO
{
    public struct FolderPath
    {
        public string Path { get; set; }

        public FolderPath(string path)
        {
            Path = PathUtils.NormalizeFolder(path);
        }

        static public FolderPath None => new FolderPath();
        public bool Defined => !string.IsNullOrWhiteSpace(Path);

        static public implicit operator string(FolderPath folderPath) => folderPath.Path;
        static public implicit operator FolderPath(string path) => new FolderPath(path);

        public override string ToString() => Path;
    }
}