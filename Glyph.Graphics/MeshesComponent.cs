using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Math;

namespace Glyph.Graphics
{
    public class MeshesComponent : GlyphComponent, IBoxedComponent, IVisualMeshProvider
    {
        public bool Visible { get; set; } = true;
        public IArea Area => MathUtils.GetBoundingBox(Meshes.SelectMany(x => x.Vertices));

        public VisualMeshCollection Meshes { get; } = new VisualMeshCollection();
        IEnumerable<IVisualMesh> IVisualMeshProvider.Meshes => Meshes;
    }
}