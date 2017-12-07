using System.Collections.Generic;

namespace Glyph.IO
{
    public struct FileType
    {
        public string DisplayName { get; set; }
        public IEnumerable<string> Extensions { get; set; }
    }
}