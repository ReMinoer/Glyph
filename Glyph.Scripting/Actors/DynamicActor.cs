using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Physics.Colliders;

namespace Glyph.Scripting.Actors
{
    public class DynamicActor : IActor
    {
        private readonly ILayerManager _layerManager;
        public IGlyphComponent Component { get; private set; }

        public IEnumerable<ICollider> Colliders
        {
            get { return Component.GetAllComponents<ICollider>(); }
        }

        public ILayerRoot LayerRoot
        {
            get { return _layerManager != null ? _layerManager.GetLayerRoot(Component) : null; }
        }

        public DynamicActor(IGlyphComponent actorComponent, ILayerManager layerManager = null)
        {
            Component = actorComponent;
            _layerManager = layerManager;
        }
    }
}