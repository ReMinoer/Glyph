using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class PrimitiveCollection : IPrimitive, IList<IPrimitive>
    {
        private readonly IList<IPrimitive> _list = new List<IPrimitive>();
        private IEnumerable Enumerable => _list;

        public bool Visible { get; set; } = true;
        public IEnumerable<Vector2> Vertices => _list.Where(x => x.VertexCount > 0).SelectMany(x => x.Vertices);
        public IEnumerable<ushort> Indices => _list.Where(x => x.IndexCount > 0).SelectMany(x => x.Indices);
        public int VertexCount => _list.Sum(x => x.VertexCount);
        public int IndexCount => _list.Sum(x => x.IndexCount);

        public IPrimitive this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            foreach (IPrimitive primitive in _list)
            {
                primitive.CopyToVertexArray(vertexArray, startIndex);
                startIndex += primitive.VertexCount;
            }
        }

        public void CopyToIndexArray(ushort[] indexArray, int startIndex)
        {
            foreach (IPrimitive primitive in _list)
            {
                primitive.CopyToIndexArray(indexArray, startIndex);
                startIndex += primitive.IndexCount;
            }
        }

        public void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            foreach (IPrimitive primitive in _list)
            {
                primitive.DrawPrimitives(graphicsDevice, verticesIndex, indicesIndex);
                verticesIndex += primitive.VertexCount;
                indicesIndex += primitive.IndexCount;
            }
        }
        
        public int Count => _list.Count;
        public void Add(IPrimitive item) => _list.Add(item);
        public void Insert(int index, IPrimitive item) => _list.Insert(index, item);
        public bool Remove(IPrimitive item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Clear() => _list.Clear();
        public bool Contains(IPrimitive item) => _list.Contains(item);
        public int IndexOf(IPrimitive item) => _list.IndexOf(item);
        public void CopyTo(IPrimitive[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        bool ICollection<IPrimitive>.IsReadOnly => _list.IsReadOnly;
        public IEnumerator<IPrimitive> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Enumerable.GetEnumerator();

    }
}