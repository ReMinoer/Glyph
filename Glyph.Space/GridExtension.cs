using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    static public class GridExtension
    {
        static public IGrid<TNewValue> Retype<TOldValue, TNewValue>(this IGrid<TOldValue> array, Func<TOldValue, TNewValue> getter)
        {
            return new RetypedGrid<TOldValue, TNewValue>(array, getter);
        }

        static public IWriteableGrid<TNewValue> Retype<TOldValue, TNewValue>(this IWriteableGrid<TOldValue> array, Func<TOldValue, TNewValue> getter, Action<TOldValue, TNewValue> setter)
        {
            return new RetypedWriteableGrid<TOldValue, TNewValue>(array, getter, setter);
        }
    }
}