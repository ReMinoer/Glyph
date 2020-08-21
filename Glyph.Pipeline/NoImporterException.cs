using System;

namespace Glyph.Pipeline
{
    public class NoImporterException : Exception
    {
        public NoImporterException(string assetPath)
            : base($"No importer found for {assetPath}")
        {
        }
    }
}