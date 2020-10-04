using System.Linq;

namespace Glyph.IO
{
    public struct FileType
    {
        public string DisplayName { get; set; }
        public string[] Extensions { get; set; }

        public FileType(params string[] extensions)
        {
            DisplayName = extensions?.First().TrimStart('.').ToUpper() + " Files";
            Extensions = extensions;
        }

        static public FileType All => new FileType(".*") { DisplayName = "All Files" };
    }
}