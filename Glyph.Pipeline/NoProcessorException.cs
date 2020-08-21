using System;

namespace Glyph.Pipeline
{
    public class NoProcessorException : Exception
    {
        public NoProcessorException(string assetPath)
            : base($"No processor found for {assetPath}")
        {
        }
    }
}