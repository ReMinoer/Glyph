using Glyph.Math;

namespace Glyph.Core
{
    public class Projection<TValue>
    {
        public TValue Value { get; set; }
        public ITransformer[] TransformerPath { get; set; }
    }
}