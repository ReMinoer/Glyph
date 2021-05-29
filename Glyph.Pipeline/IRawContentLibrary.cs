using System;
using System.Collections.Generic;

namespace Glyph.Pipeline
{
    public interface IRawContentLibrary : IContentLibrary
    {
        IEnumerable<string> GetSupportedFileExtensions(Type type);
        void CleanCookedAssets();
    }
}