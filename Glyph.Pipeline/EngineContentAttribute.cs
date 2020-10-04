using System;

namespace Glyph.Pipeline
{
    public class EngineContentAttribute : Attribute
    {
        public Type EngineContentType { get; }
        public EngineContentAttribute(Type engineContentType) { EngineContentType = engineContentType; }
    }
}