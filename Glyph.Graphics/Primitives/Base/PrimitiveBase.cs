using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives.Base
{
    public abstract class OutlinePrimitiveBase : PrimitiveBase
    {
        private Color[] _colors;
        public Color[] Colors
        {
            get => _colors;
            set
            {
                if (_colors == value)
                    return;

                _colors = value;
                DirtyColors();
            }
        }

        protected override Color GetRefreshedColor(int vertexIndex) => Colors[vertexIndex % Colors.Length];
    }

    public abstract class PrimitiveBase : IPrimitive
    {
        private VertexPositionColor[] _vertexBuffer;
        private ushort[] _indexBuffer;

        private bool _dirtyVertices = true;
        private bool _dirtyIndices = true;
        private bool _dirtyColors = true;
        private bool _dirtyVertexCount = true;
        private bool _dirtyIndexCount = true;

        public bool Visible { get; set; } = true;
        protected abstract PrimitiveType PrimitiveType { get; }

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                RefreshVertices();
                return _vertexBuffer.Select(x => x.Position.XY());
            }
        }

        public IEnumerable<ushort> Indices
        {
            get
            {
                RefreshIndices();
                return _indexBuffer.Select(x => x);
            }
        }

        private int _vertexCount;
        public int VertexCount
        {
            get
            {
                RefreshVertexCount();
                return _vertexCount;
            }
        }

        private int _indexCount;
        public int IndexCount
        {
            get
            {
                RefreshIndexCount();
                return _indexCount;
            }
        }

        private void Refresh()
        {
            RefreshVertices();
            RefreshIndices();
            RefreshColors();
        }

        private void RefreshVertexCount()
        {
            if (!_dirtyVertexCount)
                return;

            _vertexCount = GetRefreshedVertexCount();
            _dirtyVertexCount = false;
        }

        private void RefreshIndexCount()
        {
            if (!_dirtyIndexCount)
                return;

            _indexCount = GetRefreshedIndexCount();
            _dirtyIndexCount = false;
        }

        private void RefreshVertices()
        {
            if (!_dirtyVertices)
                return;
            
            _vertexBuffer = new VertexPositionColor[VertexCount];
            int i = 0;
            foreach (Vector2 vertex in GetRefreshedVertices())
            {
                _vertexBuffer[i] = new VertexPositionColor(vertex.ToVector3(), GetRefreshedColor(i));
                i++;
            }

            _dirtyVertices = false;
            _dirtyColors = false;
        }

        private void RefreshIndices()
        {
            if (!_dirtyIndices)
                return;

            _indexBuffer = new ushort[IndexCount];

            IEnumerable<ushort> refreshedIndices = GetRefreshedIndices();
            if (refreshedIndices != null)
            {
                int i = 0;
                foreach (ushort index in refreshedIndices)
                {
                    _indexBuffer[i] = index;
                    i++;
                }
            }

            _dirtyIndices = false;
        }

        private void RefreshColors()
        {
            if (!_dirtyColors)
                return;
            
            for (int i = 0; i < _vertexBuffer.Length; i++)
                _vertexBuffer[i] = new VertexPositionColor(_vertexBuffer[i].Position, GetRefreshedColor(i));

            _dirtyColors = false;
        }
        
        protected abstract int GetRefreshedVertexCount();
        protected abstract int GetRefreshedIndexCount();
        protected abstract IEnumerable<Vector2> GetRefreshedVertices();
        protected abstract IEnumerable<ushort> GetRefreshedIndices();
        protected abstract Color GetRefreshedColor(int vertexIndex);
        
        protected void DirtyVertices()
        {
            _dirtyVertices = true;
            _dirtyVertexCount = true;
        }

        protected void DirtyIndices()
        {
            _dirtyIndices = true;
            _dirtyIndexCount = true;
        }

        protected void DirtyColors()
        {
            _dirtyColors = true;
        }

        public void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            Refresh();

            for (int i = 0; i < _vertexBuffer.Length; i++)
                vertexArray[startIndex + i] = _vertexBuffer[i];
        }

        public void CopyToIndexArray(ushort[] indexArray, int startIndex)
        {
            Refresh();

            for (int i = 0; i < _indexBuffer.Length; i++)
                indexArray[startIndex + i] = _indexBuffer[i];
        }

        public void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            if (IndexCount > 0)
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType, verticesIndex, indicesIndex, GetPrimitiveCount(IndexCount));
            else
                graphicsDevice.DrawPrimitives(PrimitiveType, verticesIndex, GetPrimitiveCount(VertexCount));
        }

        private int GetPrimitiveCount(int vertexCount)
        {
            switch (PrimitiveType)
            {
                case PrimitiveType.TriangleList: return vertexCount / 3;
                case PrimitiveType.TriangleStrip: return vertexCount - 2;
                case PrimitiveType.LineList: return vertexCount / 2;
                case PrimitiveType.LineStrip: return vertexCount - 1;
                default: throw new NotSupportedException();
            }
        }
    }
}