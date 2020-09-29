using System.Collections.Generic;
using System.Linq;

namespace Glyph.IO
{
    public struct FileType
    {
        public string DisplayName { get; set; }
        public IEnumerable<string> Extensions { get; set; }

        public FileType(params string[] extensions)
        {
            DisplayName = extensions?.First().TrimStart('.').ToUpper() + " Files";
            Extensions = extensions;
        }
    }
}