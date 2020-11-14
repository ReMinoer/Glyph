using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Renderer.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Niddle.Attributes;

namespace Glyph.Graphics.Renderer
{
    public class MeshRenderer : RendererBase, ILoadContent
    {
        private readonly Func<GraphicsDevice> _graphicsDeviceFunc;
        private VertexPositionColor[] _vertexArray;
        private int[] _indexArray;
        private BasicEffect _basicEffect;

        [Populatable, ResolveTargets(ResolveTargets.Fraternal)]
        public List<IVisualMeshProvider> MeshProviders { get; } = new List<IVisualMeshProvider>();
        public IEnumerable<IVisualMesh> Meshes => MeshProviders.SelectMany(x => x.Meshes);

        protected override ISceneNode SceneNode { get; }
        protected override float DepthProtected => SceneNode.Depth;

        public override IArea Area
        {
            get
            {
                TopLeftRectangle boundingBox = MathUtils.GetBoundingBox(Meshes.SelectMany(x => x.Vertices));
                return SceneNode.Transform(boundingBox);
            }
        }

        public MeshRenderer(SceneNode sceneNode, Func<GraphicsDevice> graphicsDeviceFunc)
        {
            _graphicsDeviceFunc = graphicsDeviceFunc;
            SceneNode = sceneNode;
        }

        public Task LoadContent(IContentLibrary contentLibrary)
        {
            GraphicsDevice graphicsDevice = _graphicsDeviceFunc();

            _basicEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true
            };

            RefreshBuffers();

            return Task.CompletedTask;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void RefreshBuffers()
        {
            int totalVertexCount = Meshes.Sum(x => x.VertexCount);
            if (totalVertexCount == 0)
            {
                _vertexArray = null;
                _indexArray = null;
                return;
            }

            // Resize vertex array if necessary
            if (totalVertexCount != _vertexArray?.Length)
                _vertexArray = new VertexPositionColor[totalVertexCount];

            // Fill vertex array
            int i = 0;
            foreach (IVisualMesh mesh in Meshes)
            {
                mesh.CopyToVertexArray(_vertexArray, i);
                i += mesh.VertexCount;
            }

            // Skip index array if none provided
            int totalIndexCount = Meshes.Sum(x => x.IndexCount);
            if (totalIndexCount == 0)
            {
                _indexArray = null;
                return;
            }

            // Resize index buffer if necessary
            if (totalIndexCount != _indexArray?.Length)
                _indexArray = new int[totalIndexCount];

            // Fill index array
            i = 0;
            foreach (IVisualMesh mesh in Meshes)
            {
                mesh.CopyToIndexArray(_indexArray, i);
                i += mesh.IndexCount;
            }
        }

        protected override void Render(IDrawer drawer)
        {
            RefreshBuffers();
            if (_vertexArray == null)
                return;

            drawer.SpriteBatchStack.Push(null);

            // Configure basic effect
            Quad rect = drawer.DisplayedRectangle;

            _basicEffect.World = SceneNode.Matrix;
            _basicEffect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, 0, float.MaxValue);

            // Draw meshes
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                int verticesIndex = 0;
                int indicesIndex = 0;
                foreach (IVisualMesh mesh in Meshes)
                {
                    if (mesh.Visible)
                    {
                        if (mesh.IndexCount > 0)
                            drawer.GraphicsDevice.DrawUserIndexedPrimitives(mesh.Type, _vertexArray, verticesIndex, mesh.VertexCount, _indexArray, indicesIndex, GetPrimitiveCount(mesh.Type, mesh.IndexCount));
                        else
                            drawer.GraphicsDevice.DrawUserPrimitives(mesh.Type, _vertexArray, verticesIndex, GetPrimitiveCount(mesh.Type, mesh.VertexCount));
                    }

                    verticesIndex += mesh.VertexCount;
                    indicesIndex += mesh.IndexCount;
                }
            }

            drawer.SpriteBatchStack.Pop();
        }

        static private int GetPrimitiveCount(PrimitiveType primitiveType, int vertexCount)
        {
            switch (primitiveType)
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