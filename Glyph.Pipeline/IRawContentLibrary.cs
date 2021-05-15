using System;
using System.Collections.Generic;

namespace Glyph.Pipeline
{
    public interface IRawContentLibrary : IContentLibrary
    {
        ICollection<string> GetRawFilesPaths(string assetPath);
        IEnumerable<string> GetSupportedFileExtensions(Type type);
    }
}