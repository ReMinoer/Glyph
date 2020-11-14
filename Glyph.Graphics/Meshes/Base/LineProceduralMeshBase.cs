using System.Collections.Generic;
using Glyph.Graphics.Meshes.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class LineProceduralMeshBase : ProceduralMeshBase
    {
        protected abstract bool IsStrip { get; }
        public override sealed PrimitiveType Type => IsStrip ? PrimitiveType.LineStrip : PrimitiveType.LineList;

        public Color[] Colors { get; set; }
        protected override Color GetColor(int vertexIndex) => Colors[vertexIndex % Colors.Length];

        protected override IEnumerable<Vector2> GetRefreshedTextureCoordinates() => MeshHelpers.GetOrthographicTextureCoordinates(this);
    }
}