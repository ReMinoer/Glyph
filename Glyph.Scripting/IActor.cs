using System.Collections.Generic;
using Glyph.Composition.Layers;
using Glyph.Physics.Colliders;

namespace Glyph.Scripting
{
    public interface IActor
    {
        IEnumerable<ICollider> Colliders { get; }
        ILayerRoot LayerRoot { get; }
    }
}