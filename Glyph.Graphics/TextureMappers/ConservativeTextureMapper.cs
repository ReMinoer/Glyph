using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.TextureMappers
{
    public class ConservativeTextureMapper : ITextureMapper
    {
        private readonly ISpriteSource _spriteSource;
        public event EventHandler RefreshRequested;

        public ConservativeTextureMapper(ISpriteSource spriteSource)
        {
            _spriteSource = spriteSource;
            _spriteSource.Loaded += OnSourceLoaded;
        }

        private void OnSourceLoaded(ISpriteSource obj)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        public Vector2[] GetVertexTextureCoordinates(IReadOnlyList<Vector2> vertices)
        {
            var textureCoordinates = new Vector2[vertices.Count];

            Texture2D texture = _spriteSource.Texture;
            if (texture != null)
            {
                Vector2 size = texture.Size();
                for (int i = 0; i < textureCoordinates.Length; i++)
                    textureCoordinates[i] = vertices[i] / size;
            }

            return textureCoordinates;
        }
    }
}