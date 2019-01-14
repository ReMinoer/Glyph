using System.Collections.Generic;
using Diese;

namespace Glyph
{
    public class RepresentativeEqualityComparer<T> : IEqualityComparer<IRepresentative<T>>
    {
        public bool Equals(IRepresentative<T> x, IRepresentative<T> y)
        {
            return (x?.Represent(y) ?? false) || (y?.Represent(x) ?? false);
        }

        public int GetHashCode(IRepresentative<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}