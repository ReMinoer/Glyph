using System.Collections.Generic;

namespace Glyph.Graphics
{
    public interface IVisualMeshProvider
    {
        IEnumerable<IVisualMesh> Meshes { get; }
    }
}