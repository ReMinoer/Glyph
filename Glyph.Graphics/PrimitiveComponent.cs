using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Math;

namespace Glyph.Graphics
{
    public class PrimitivesComponent : GlyphComponent, IBoxedComponent, IPrimitiveProvider
    {
        public bool Visible { get; set; } = true;
        public IArea Area => MathUtils.GetBoundingBox(Primitives.SelectMany(x => x.Vertices));

        public PrimitiveCollection Primitives { get; } = new PrimitiveCollection();
        IEnumerable<IPrimitive> IPrimitiveProvider.Primitives => Primitives;

    }
}