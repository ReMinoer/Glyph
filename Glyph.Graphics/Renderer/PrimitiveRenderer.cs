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
    public class PrimitiveRenderer : RendererBase, ILoadContent
    {
        private readonly Func<GraphicsDevice> _graphicsDeviceFunc;
        private DynamicVertexBuffer _vertexBuffer;
        private DynamicIndexBuffer _indexBuffer;
        private VertexPositionColor[] _vertexArray;
        private ushort[] _indexArray;
        private BasicEffect _basicEffect;

        [Populatable, ResolveTargets(ResolveTargets.Fraternal)]
        public List<IPrimitive> Primitives { get; } = new List<IPrimitive>();
        
        public override IArea Area => MathUtils.GetBoundingBox(Primitives.SelectMany(x => x.Vertices));
        protected override ISceneNode SceneNode { get; }
        protected override float DepthProtected => SceneNode.Depth;

        public PrimitiveRenderer(SceneNode sceneNode, Func<GraphicsDevice> graphicsDeviceFunc)
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

        public override void Dispose()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void RefreshBuffers()
        {
            IEnumerable<IPrimitive> visiblePrimitives = Primitives.Where(x => x.Visible);
            int totalVertexCount = visiblePrimitives.Sum(x => x.VertexCount);
            if (totalVertexCount == 0)
            {
                _vertexArray = null;
                _vertexBuffer = null;
                _indexArray = null;
                _indexBuffer = null;
                return;
            }

            GraphicsDevice graphicsDevice = _graphicsDeviceFunc();

            // Resize vertex array if necessary
            if (totalVertexCount != _vertexArray?.Length)
                _vertexArray = new VertexPositionColor[totalVertexCount];

            // Fill vertex array
            int i = 0;
            foreach (IPrimitive primitive in visiblePrimitives)
            {
                primitive.CopyToVertexArray(_vertexArray, i);
                i += primitive.VertexCount;
            }

            // Resize vertex buffer if necessary
            if (_vertexBuffer == null || _vertexArray.Length > _vertexBuffer.VertexCount)
                _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColor), _vertexArray.Length, BufferUsage.WriteOnly);

            // Set array to vertex buffer
            _vertexBuffer.SetData(_vertexArray);

            // Skip index array if none provided
            int totalIndexCount = visiblePrimitives.Sum(x => x.IndexCount);
            if (totalIndexCount == 0)
            {
                _indexArray = null;
                _indexBuffer = null;
                return;
            }

            // Resize index buffer if necessary
            if (totalIndexCount != _indexArray?.Length)
            {
                _indexArray = new ushort[totalIndexCount];
                _indexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), _indexArray.Length, BufferUsage.WriteOnly);
            }

            // Fill index array
            i = 0;
            foreach (IPrimitive primitive in visiblePrimitives)
            {
                primitive.CopyToIndexArray(_indexArray, i);
                i += primitive.IndexCount;
            }

            // Resize vertex buffer if necessary
            if (_indexBuffer == null || _indexArray.Length > _indexBuffer.IndexCount)
                _indexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), _indexArray.Length, BufferUsage.WriteOnly);

            // Set array to index buffer
            _indexBuffer.SetData(_indexArray);
        }

        protected override void Render(IDrawer drawer)
        {
            RefreshBuffers();

            drawer.SpriteBatchStack.Push(null);

            // Set vertex and index buffers
            drawer.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            if (_vertexBuffer != null)
                drawer.GraphicsDevice.Indices = _indexBuffer;

            // Configure basic effect
            Quad rect = drawer.DisplayedRectangle;

            _basicEffect.World = SceneNode.Matrix;
            _basicEffect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, 0, float.MaxValue);
            
            // Draw primitives
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                int verticesIndex = 0;
                int indicesIndex = 0;
                foreach (IPrimitive primitive in Primitives.Where(x => x.Visible))
                {
                    primitive.DrawPrimitives(drawer.GraphicsDevice, verticesIndex, indicesIndex);
                    verticesIndex += primitive.VertexCount;
                    indicesIndex += primitive.IndexCount;
                }
            }

            drawer.GraphicsDevice.SetVertexBuffer(null);
            drawer.GraphicsDevice.Indices = null;
            drawer.SpriteBatchStack.Pop();
        }
    }
}