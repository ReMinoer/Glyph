using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Physics.Colliders;

namespace Glyph.Scripting.Actors
{
    public class DelegateActor : IActor
    {
        private readonly ILayerManager _layerManager;
        public Func<IGlyphComponent> ComponentFunc { get; private set; }

        public IEnumerable<ICollider> Colliders
        {
            get { return ComponentFunc().GetAllComponents<ICollider>(); }
        }

        public ILayerRoot LayerRoot
        {
            get { return _layerManager != null ? _layerManager.GetLayerRoot(ComponentFunc()) : null; }
        }

        public DelegateActor(Func<IGlyphComponent> componentFunc, ILayerManager layerManager = null)
        {
            ComponentFunc = componentFunc;
            _layerManager = layerManager;
        }
    }
}