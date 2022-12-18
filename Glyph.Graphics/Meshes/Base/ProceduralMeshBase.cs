using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class ProceduralMeshBase : TexturableMeshBase
    {
        private bool _dirtyCaches = true;
        private readonly List<Vector2> _vertexCache;
        private readonly List<int> _indexCache;
        private readonly IReadOnlyList<Vector2> _readOnlyVertexCache;
        private readonly IReadOnlyList<int> _readOnlyIndexCache;

        protected override sealed IReadOnlyList<Vector2> ReadOnlyVertices
        {
            get
            {
                RefreshCache();
                return _readOnlyVertexCache;
            }
        }

        protected override sealed IList<int> ReadOnlyIndices
        {
            get
            {
                RefreshCache();
                return _indexCache;
            }
        }

        public override sealed int VertexCount
        {
            get
            {
                RefreshCache();
                return _readOnlyVertexCache.Count;
            }
        }

        public override sealed int TriangulationIndexCount
        {
            get
            {
                RefreshCache();
                return _readOnlyIndexCache.Count;
            }
        }

        protected ProceduralMeshBase()
        {
            _vertexCache = new List<Vector2>();
            _indexCache = new List<int>();

            _readOnlyVertexCache = new ReadOnlyCollection<Vector2>(_vertexCache);
            _readOnlyIndexCache = new ReadOnlyCollection<int>(_indexCache);
        }

        protected override sealed void RefreshVertexCache() => RefreshCache();

        private void RefreshCache()
        {
            if (!_dirtyCaches)
                return;

            RefreshCache(_vertexCache, _indexCache);
            _dirtyCaches = false;
        }

        protected abstract void RefreshCache(List<Vector2> vertices, List<int> indices);

        protected void DirtyCaches()
        {
            _dirtyCaches = true;
            DirtyTextureCoordinates();
        }

        protected void SetAndDirtyCachesOnChanged<T>(ref T field, T value)
        {
            if (Equals(field, value))
                return;

            field = value;
            DirtyCaches();
        }
    }

    public abstract class ComplexProceduralMeshBase : TexturableMeshBase
    {
        private bool _dirtyCaches = true;
        private readonly IndexedVertexCollection _indexedVertexCache;

        protected override sealed IReadOnlyList<Vector2> ReadOnlyVertices
        {
            get
            {
                RefreshCache();
                return _indexedVertexCache.Vertices;
            }
        }

        protected override sealed IList<int> ReadOnlyIndices
        {
            get
            {
                RefreshCache();
                return _indexedVertexCache.Indices;
            }
        }

        public override sealed int VertexCount
        {
            get
            {
                RefreshCache();
                return _indexedVertexCache.Vertices.Count;
            }
        }

        public override sealed int TriangulationIndexCount
        {
            get
            {
                RefreshCache();
                return _indexedVertexCache.Indices.Count;
            }
        }

        protected ComplexProceduralMeshBase()
        {
            _indexedVertexCache = new IndexedVertexCollection();
        }

        protected override sealed void RefreshVertexCache() => RefreshCache();

        private void RefreshCache()
        {
            if (!_dirtyCaches)
                return;

            RefreshCache(_indexedVertexCache);
            _dirtyCaches = false;
        }

        protected abstract void RefreshCache(IndexedVertexCollection indexedVertices);

        protected void DirtyCaches()
        {
            _dirtyCaches = true;
            DirtyTextureCoordinates();
        }

        protected class IndexedVertexCollection : ICollection<Vector2>
        {
            private readonly List<Vector2> _vertices;
            private readonly List<int> _indices;
            private readonly Dictionary<Vector2, int> _sharedIndices;
            
            public IReadOnlyList<Vector2> Vertices { get; }
            public IList<int> Indices { get; }

            public IndexedVertexCollection()
            {
                _vertices = new List<Vector2>();
                Vertices = _vertices.AsReadOnly();

                _indices = new List<int>();
                Indices = _indices.AsReadOnly();

                _sharedIndices = new Dictionary<Vector2, int>();
            }

            public void Add(Vector2 vertex)
            {
                // TODO: Use a partitioned space.
                if (!_sharedIndices.TryGetValue(vertex, out int index))
                {
                    index = _vertices.Count;
                    _vertices.Add(vertex);
                    _sharedIndices.Add(vertex, index);
                }

                _indices.Add(index);
            }
            
            public void Clear()
            {
                _vertices.Clear();
                _indices.Clear();
                _sharedIndices.Clear();
            }

            bool ICollection<Vector2>.Contains(Vector2 item) => _vertices.Contains(item);
            void ICollection<Vector2>.CopyTo(Vector2[] array, int arrayIndex) { _vertices.CopyTo(array, arrayIndex); }
            bool ICollection<Vector2>.Remove(Vector2 item) => _vertices.Remove(item);

            int ICollection<Vector2>.Count => _vertices.Count;
            bool ICollection<Vector2>.IsReadOnly => false;

            IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator() => _vertices.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_vertices).GetEnumerator();
        }
    }
}