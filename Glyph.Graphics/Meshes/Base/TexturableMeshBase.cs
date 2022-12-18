using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Graphics.TextureMappers;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class TexturableMeshBase : MeshBase
    {
        private Vector2[] _textureCoordinatesCache;
        private IReadOnlyList<Vector2> _readOnlyTextureCoordinatesCache;
        private bool _dirtyTextureCoordinates = true;

        protected override IReadOnlyList<Vector2> ReadOnlyTextureCoordinates
        {
            get
            {
                RefreshTextureCoordinates();
                return _readOnlyTextureCoordinatesCache;
            }
        }

        private ITextureMapper _textureMapper;
        public ITextureMapper TextureMapper
        {
            get => _textureMapper;
            set
            {
                if (_textureMapper == value)
                    return;

                if (_textureMapper != null)
                    _textureMapper.RefreshRequested -= OnRefreshRequested;

                _textureMapper = value;
                _dirtyTextureCoordinates = true;

                if (_textureMapper != null)
                    _textureMapper.RefreshRequested += OnRefreshRequested;

                void OnRefreshRequested(object sender, EventArgs e) => DirtyTextureCoordinates();
            }
        }

        protected void RefreshTextureCoordinates()
        {
            RefreshVertexCache();
            if (!_dirtyTextureCoordinates)
                return;

            ITextureMapper textureMapper = TextureMapper ?? NormalizedTextureMapper.Instance;
            _textureCoordinatesCache = textureMapper.GetVertexTextureCoordinates(ReadOnlyVertices);

            _readOnlyTextureCoordinatesCache = new ReadOnlyCollection<Vector2>(_textureCoordinatesCache);
            _dirtyTextureCoordinates = false;
        }

        protected abstract void RefreshVertexCache();

        protected void DirtyTextureCoordinates()
        {
            _dirtyTextureCoordinates = true;
            DirtyDrawVerticesCache();
        }
    }
}