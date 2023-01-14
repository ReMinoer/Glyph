using System.Collections.Generic;
using System.Linq;
using Diese.Collections.ReadOnly;
using Glyph.Math;

namespace Glyph.Core
{
    public class Projection<TValue>
    {
        public TValue Value { get; }
        public IReadOnlyList<ITransformer> TransformerPath { get; }
        public IReadOnlyList<ITransformer> GraphPath { get; }

        public Projection(TValue value, IEnumerable<ITransformer> transformerPath, IEnumerable<ITransformer> graphPath)
        {
            Value = value;
            TransformerPath = new ReadOnlyList<ITransformer>(transformerPath.ToArray());
            GraphPath = new ReadOnlyList<ITransformer>(graphPath.ToArray());
        }
    }
}