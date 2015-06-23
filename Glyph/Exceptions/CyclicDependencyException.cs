using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Exceptions
{
    public class CyclicDependencyException : GlyphException
    {
        private const string MessageByType = "A cyclic dependency has been detected ! (Stack : {0})";

        public CyclicDependencyException(IEnumerable<Type> typeStack)
            : base(MessageByType, typeStack.Aggregate("", (x,y) => x + ", " + y.Name))
        {
        }

        public CyclicDependencyException(IEnumerable<object> objectStack)
            : this(objectStack.Select(x => x.GetType()))
        {
        }
    }
}