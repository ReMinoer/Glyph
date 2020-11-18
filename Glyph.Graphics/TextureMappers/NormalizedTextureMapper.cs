using System;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.TextureMappers
{
    public class NormalizedTextureMapper : ITextureMapper
    {
        static private NormalizedTextureMapper _instance;
        static public NormalizedTextureMapper Instance => _instance ?? (_instance = new NormalizedTextureMapper());

        public event EventHandler RefreshRequested;

        private NormalizedTextureMapper()
        {
        }

        public Vector2[] GetVertexTextureCoordinates(IReadOnlyList<Vector2> vertices)
        {
            var textureCoordinates = new Vector2[vertices.Count];

            TopLeftRectangle boundingBox = MathUtils.GetBoundingBox(vertices);
            for (int i = 0; i < textureCoordinates.Length; i++)
                textureCoordinates[i] = vertices[i].Rescale(boundingBox, new TopLeftRectangle(0, 0, 1, 1));

            return textureCoordinates;
        }
    }
}