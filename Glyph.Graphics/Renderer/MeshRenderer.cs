using System;
using System.Collections.Generic;
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
        private VertexPositionColorTexture[] _vertexArray;
        private int[] _indexArray;
        private BasicEffect _defaultEffect;

        [Populatable, ResolveTargets(ResolveTargets.Fraternal)]
        public List<IVisualMeshProvider> MeshProviders { get; } = new List<IVisualMeshProvider>();
        public IEnumerable<IVisualMesh> Meshes => MeshProviders.SelectMany(x => x.Meshes);

        [Resolvable]
        public ISpriteSource TextureSource { get; set; }

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

            _defaultEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true
            };

            RefreshBuffers();

            return Task.CompletedTask;
        }

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
                _vertexArray = new VertexPositionColorTexture[totalVertexCount];

            // Fill vertex array
            int i = 0;
            foreach (IVisualMesh mesh in Meshes)
            {
                mesh.CopyToVertexArray(_vertexArray, i);
                i += mesh.VertexCount;
            }

            // Skip index array if none provided
            int totalIndexCount = Meshes.Sum(x => x.TriangulationIndexCount);
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
                i += mesh.TriangulationIndexCount;
            }
        }

        protected override void Render(IDrawer drawer)
        {
            RefreshBuffers();
            if (_vertexArray == null)
                return;

            GraphicsDevice graphicsDevice = drawer.GraphicsDevice;
            Quad rect = drawer.DisplayedRectangle;

            // Configure default effect matrices
            _defaultEffect.World = SceneNode.Matrix;
            _defaultEffect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _defaultEffect.Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, 0, float.MaxValue);

            // Get texture
            Texture2D texture = TextureSource?.Texture;
            _defaultEffect.TextureEnabled = texture != null;

            // Configure sampler states
            drawer.SpriteBatchStack.Push(null);
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            int verticesIndex = 0;
            int indicesIndex = 0;
            Effect currentEffect = null;
            EffectPass currentPass = null;

            foreach (IVisualMesh mesh in Meshes)
            {
                // If mesh not visible, move indexes and go to next
                if (!mesh.Visible)
                {
                    verticesIndex += mesh.VertexCount;
                    indicesIndex += mesh.TriangulationIndexCount;
                    continue;
                }

                bool meshIndexed = mesh.TriangulationIndexCount > 0;

                foreach (IVisualMeshPart part in mesh.Parts)
                {
                    Effect effect = part.Effect;
                    if (effect == null)
                    {
                        // If no effect provided, use default effect
                        effect = _defaultEffect;
                    }
                    else if (effect != currentEffect)
                    {
                        // Configure effect matrices 
                        IEffectMatrices effectMatrices = part.EffectMatrices;
                        if (effectMatrices != null)
                        {
                            effectMatrices.World = _defaultEffect.World;
                            effectMatrices.View = _defaultEffect.View;
                            effectMatrices.Projection = _defaultEffect.Projection;
                        }

                        currentEffect = effect;
                    }

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        if (pass != currentPass)
                        {
                            // Apply effect pass
                            pass.Apply();

                            // Set texture
                            if (texture != null)
                                graphicsDevice.Textures[0] = texture;

                            currentPass = pass;
                        }

                        // Draw primitives
                        if (meshIndexed)
                        {
                            int primitiveCount = GetPrimitiveCount(mesh.Type, part.TriangulationIndexCount);

                            graphicsDevice.DrawUserIndexedPrimitives(mesh.Type, _vertexArray, verticesIndex, mesh.VertexCount, _indexArray, indicesIndex, primitiveCount);
                            indicesIndex += part.TriangulationIndexCount;
                        }
                        else
                        {
                            int primitiveCount = GetPrimitiveCount(mesh.Type, part.VertexCount);

                            graphicsDevice.DrawUserPrimitives(mesh.Type, _vertexArray, verticesIndex, primitiveCount);
                            verticesIndex += part.VertexCount;
                        }
                    }
                }

                // If mesh indexed, move vertices index after all parts have been drawn.
                if (meshIndexed)
                    verticesIndex += mesh.VertexCount;
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