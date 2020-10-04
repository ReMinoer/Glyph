using System;
using System.Collections.Generic;

namespace Glyph.Pipeline
{
    public interface IRawContentLibrary
    {
        ICollection<string> GetRawFilesPaths(string assetPath);
        IEnumerable<string> GetSupportedFileExtensions(Type type);
    }
}