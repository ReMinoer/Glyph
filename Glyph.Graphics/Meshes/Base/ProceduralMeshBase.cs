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

        protected override IReadOnlyList<Vector2> ReadOnlyVertices
        {
            get
            {
                RefreshCache();
                return _readOnlyVertexCache;
            }
        }

        protected override IReadOnlyList<int> ReadOnlyIndices
        {
            get
            {
                RefreshCache();
                return _readOnlyIndexCache;
            }
        }

        public override int VertexCount
        {
            get
            {
                RefreshCache();
                return _readOnlyVertexCache.Count;
            }
        }

        public override int TriangulationIndexCount
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
}