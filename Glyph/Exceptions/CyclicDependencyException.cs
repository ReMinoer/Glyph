using System;

namespace Glyph.Exceptions
{
    public class CyclicDependencyException : GlyphException
    {
        private const string MessageByObject = "A cyclic dependency has been detected ! Init by object : {0}";
        private const string MessageByType = "A cyclic dependency has been detected ! Init by type : {0}";

        public CyclicDependencyException(object initObject)
            : base(MessageByObject, initObject)
        {
        }

        public CyclicDependencyException(Type initType)
            : base(MessageByType, initType.Name)
        {
        }
    }
}