using System;
using System.Collections.Generic;
using System.Linq;

namespace Glyph.Core.Scheduler.Base
{
    public class CyclicDependencyException : Exception
    {
        private const string MessageByType = "A cyclic dependency has been detected ! (Stack : {0})";

        public CyclicDependencyException(IEnumerable<Type> typeStack)
            : base(string.Format(MessageByType, typeStack.Aggregate("", (x, y) => x + ", " + y.Name)))
        {
        }

        public CyclicDependencyException(IEnumerable<object> objectStack)
            : this(objectStack.Select(x => x.GetType()))
        {
        }
    }
}