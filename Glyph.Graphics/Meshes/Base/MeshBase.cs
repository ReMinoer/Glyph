using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes.Base
{
    public abstract class MeshBase : IVisualMesh, IVisualMeshPart, IVisualMeshProvider
    {
        public bool Visible { get; set; } = true;

        public abstract PrimitiveType Type { get; }
        public IEnumerable<Vector2> Vertices => ReadOnlyVertices;
        public IEnumerable<Vector2> TextureCoordinates => ReadOnlyTextureCoordinates;
        public IEnumerable<int> TriangulationIndices => ReadOnlyIndices;
        protected abstract IReadOnlyList<Vector2> ReadOnlyVertices { get; }
        protected abstract IReadOnlyList<Vector2> ReadOnlyTextureCoordinates { get; }
        protected abstract IReadOnlyList<int> ReadOnlyIndices { get; }
        public virtual int VertexCount => ReadOnlyVertices?.Count ?? 0;
        public virtual int TriangulationIndexCount => ReadOnlyIndices?.Count ?? 0;

        public Effect Effect { get; private set; }
        public IEffectMatrices EffectMatrices { get; private set; }

        private readonly IVisualMeshPart[] _parts;
        public IEnumerable<IVisualMeshPart> Parts => _parts;

        private readonly IVisualMesh[] _meshes;
        IEnumerable<IVisualMesh> IVisualMeshProvider.Meshes => _meshes;

        protected MeshBase()
        {
            _meshes = new IVisualMesh[]{this};
            _parts = new IVisualMeshPart[]{this};
        }

        public virtual void CopyToVertexArray(VertexPosition[] vertexArray, int startIndex)
        {
            for (int i = 0; i < VertexCount; i++)
                vertexArray[startIndex + i] = new VertexPosition(ReadOnlyVertices[i].ToVector3());
        }

        public virtual void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            for (int i = 0; i < VertexCount; i++)
                vertexArray[startIndex + i] = new VertexPositionColor(ReadOnlyVertices[i].ToVector3(), GetColor(i));
        }

        public virtual void CopyToVertexArray(VertexPositionColorTexture[] vertexArray, int startIndex)
        {
            for (int i = 0; i < VertexCount; i++)
                vertexArray[startIndex + i] = new VertexPositionColorTexture(ReadOnlyVertices[i].ToVector3(), GetColor(i), ReadOnlyTextureCoordinates[i]);
        }

        public virtual void CopyToVertexArray(VertexPositionTexture[] vertexArray, int startIndex)
        {
            for (int i = 0; i < VertexCount; i++)
                vertexArray[startIndex + i] = new VertexPositionTexture(ReadOnlyVertices[i].ToVector3(), ReadOnlyTextureCoordinates[i]);
        }

        public virtual void CopyToIndexArray(int[] indexArray, int startIndex)
        {
            for (int i = 0; i < TriangulationIndexCount; i++)
                indexArray[startIndex + i] = ReadOnlyIndices[i];
        }

        public void SetEffect<TEffect>(TEffect effect)
            where TEffect : Effect, IEffectMatrices
        {
            Effect = effect;
            EffectMatrices = effect;
        }

        public void SetEffect(Effect effect)
        {
            Effect = effect;
            EffectMatrices = null;
        }

        protected abstract Color GetColor(int vertexIndex);
    }
}