using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class ProceduralMeshBase : MeshBase
    {
        private Vector2[] _vertexCache;
        private Vector2[] _textureCoordinatesCache;
        private int[] _indexCache;
        private IReadOnlyList<Vector2> _readOnlyVertexCache;
        private IReadOnlyList<Vector2> _readOnlyTextureCoordinatesCache;
        private IReadOnlyList<int> _readOnlyIndexCache;

        private bool _dirtyVertices = true;
        private bool _dirtyIndices = true;
        private bool _dirtyVertexCount = true;
        private bool _dirtyIndexCount = true;

        protected override IReadOnlyList<Vector2> ReadOnlyVertices
        {
            get
            {
                RefreshVertices();
                return _readOnlyVertexCache;
            }
        }

        protected override IReadOnlyList<Vector2> ReadOnlyTextureCoordinates
        {
            get
            {
                RefreshVertices();
                return _readOnlyTextureCoordinatesCache;
            }
        }

        protected override IReadOnlyList<int> ReadOnlyIndices
        {
            get
            {
                RefreshIndices();
                return _readOnlyIndexCache;
            }
        }

        private int _vertexCount;
        public override int VertexCount
        {
            get
            {
                RefreshVertexCount();
                return _vertexCount;
            }
        }

        private int _indexCount;
        public override int IndexCount
        {
            get
            {
                RefreshIndexCount();
                return _indexCount;
            }
        }

        protected virtual bool Refresh()
        {
            return RefreshVertexCount()
                | RefreshIndexCount()
                | RefreshVertices()
                | RefreshIndices();
        }

        private bool RefreshVertexCount()
        {
            if (!_dirtyVertexCount)
                return false;

            _vertexCount = GetRefreshedVertexCount();
            _dirtyVertexCount = false;

            return true;
        }

        private bool RefreshIndexCount()
        {
            if (!_dirtyIndexCount)
                return false;

            _indexCount = GetRefreshedIndexCount();
            _dirtyIndexCount = false;

            return true;
        }

        protected virtual bool RefreshVertices()
        {
            if (!_dirtyVertices)
                return false;

            int i = 0;
            _vertexCache = new Vector2[VertexCount];
            foreach (Vector2 vertex in GetRefreshedVertices())
            {
                _vertexCache[i] = vertex;
                i++;
            }

            _readOnlyVertexCache = new ReadOnlyCollection<Vector2>(_vertexCache);
            _dirtyVertices = false;

            i = 0;
            _textureCoordinatesCache = new Vector2[VertexCount];
            foreach (Vector2 vertex in GetRefreshedTextureCoordinates())
            {
                _textureCoordinatesCache[i] = vertex;
                i++;
            }

            _readOnlyTextureCoordinatesCache = new ReadOnlyCollection<Vector2>(_textureCoordinatesCache);
            return true;
        }

        private bool RefreshIndices()
        {
            if (!_dirtyIndices)
                return false;

            _indexCache = new int[IndexCount];

            IEnumerable<int> refreshedIndices = GetRefreshedIndices();
            if (refreshedIndices != null)
            {
                int i = 0;
                foreach (int index in refreshedIndices)
                {
                    _indexCache[i] = index;
                    i++;
                }
            }

            _readOnlyIndexCache = new ReadOnlyCollection<int>(_indexCache);
            _dirtyIndices = false;
            return true;
        }

        protected abstract int GetRefreshedVertexCount();
        protected abstract int GetRefreshedIndexCount();
        protected abstract IEnumerable<Vector2> GetRefreshedVertices();
        protected abstract IEnumerable<Vector2> GetRefreshedTextureCoordinates();
        protected abstract IEnumerable<int> GetRefreshedIndices();

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

        public override void CopyToVertexArray(VertexPosition[] vertexArray, int startIndex)
        {
            Refresh();
            base.CopyToVertexArray(vertexArray, startIndex);
        }

        public override void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            Refresh();
            base.CopyToVertexArray(vertexArray, startIndex);
        }

        public override void CopyToVertexArray(VertexPositionColorTexture[] vertexArray, int startIndex)
        {
            Refresh();
            base.CopyToVertexArray(vertexArray, startIndex);
        }

        public override void CopyToVertexArray(VertexPositionTexture[] vertexArray, int startIndex)
        {
            Refresh();
            base.CopyToVertexArray(vertexArray, startIndex);
        }

        public override void CopyToIndexArray(int[] indexArray, int startIndex)
        {
            Refresh();
            base.CopyToIndexArray(indexArray, startIndex);
        }
    }
}