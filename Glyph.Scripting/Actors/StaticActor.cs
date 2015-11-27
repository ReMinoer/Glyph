using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Physics.Colliders;

namespace Glyph.Scripting.Actors
{
    public class StaticActor : IActor
    {
        public IEnumerable<ICollider> Colliders { get; private set; }
        public ILayerRoot LayerRoot { get; private set; }

        public StaticActor(IGlyphComponent actorComponent, ILayerManager layerManager = null)
        {
            Colliders = actorComponent.GetAllComponents<ICollider>();

            if (layerManager != null)
                LayerRoot = layerManager.GetLayerRoot(actorComponent);
        }
    }
}