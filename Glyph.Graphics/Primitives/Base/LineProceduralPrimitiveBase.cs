using System.Collections.Generic;
using Glyph.Graphics.Primitives.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives.Base
{
    public abstract class LineProceduralPrimitiveBase : ProceduralPrimitiveBase
    {
        protected abstract bool IsStrip { get; }
        public override sealed PrimitiveType Type => IsStrip ? PrimitiveType.LineStrip : PrimitiveType.LineList;

        public Color[] Colors { get; set; }
        protected override Color GetColor(int vertexIndex) => Colors[vertexIndex % Colors.Length];

        protected override IEnumerable<Vector2> GetRefreshedTextureCoordinates() => PrimitiveHelpers.GetOrthographicTextureCoordinates(this);
    }
}