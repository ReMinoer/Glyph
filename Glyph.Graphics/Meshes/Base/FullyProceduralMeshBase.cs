using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class FullyProceduralMeshBase : TexturableMeshBase
    {
        private Vector2[] _vertexCache;
        private int[] _indexCache;
        private IReadOnlyList<Vector2> _readOnlyVertexCache;
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
        public override int TriangulationIndexCount
        {
            get
            {
                RefreshIndexCount();
                return _indexCount;
            }
        }

        private void Refresh()
        {
            RefreshVertexCount();
            RefreshIndexCount();
            RefreshVertices();
            RefreshIndices();
            RefreshTextureCoordinates();
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

        protected override sealed void RefreshVertexCache() => RefreshVertices();
        private void RefreshVertices()
        {
            if (!_dirtyVertices)
                return;

            int i = 0;
            _vertexCache = new Vector2[VertexCount];
            foreach (Vector2 vertex in GetRefreshedVertices())
            {
                _vertexCache[i] = vertex;
                i++;
            }

            _readOnlyVertexCache = new ReadOnlyCollection<Vector2>(_vertexCache);
            _dirtyVertices = false;
        }

        private void RefreshIndices()
        {
            if (!_dirtyIndices)
                return;

            _indexCache = new int[TriangulationIndexCount];

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
        }

        protected abstract int GetRefreshedVertexCount();
        protected abstract int GetRefreshedIndexCount();
        protected abstract IEnumerable<Vector2> GetRefreshedVertices();
        protected abstract IEnumerable<int> GetRefreshedIndices();

        protected void DirtyVertices()
        {
            _dirtyVertices = true;
            _dirtyVertexCount = true;
            DirtyTextureCoordinates();
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