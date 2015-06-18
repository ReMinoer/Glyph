using System;

namespace Glyph.Exceptions
{
    public abstract class GlyphException : Exception
    {
        protected GlyphException()
        {
        }

        protected GlyphException(string message)
            : base(message)
        {
        }

        protected GlyphException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }
    }
}