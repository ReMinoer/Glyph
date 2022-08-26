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

            SubscribeDepthChanged(sceneNode);
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            GraphicsDevice graphicsDevice = _graphicsDeviceFunc();

            _defaultEffect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true
            };

            RefreshBuffers();
        }

        public Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            LoadContent(contentLibrary);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _defaultEffect?.Dispose();
            base.Dispose();
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

        public override void Draw(IDrawer drawer)
        {
            RefreshBuffers();
            if (_vertexArray == null)
                return;

            GraphicsDevice graphicsDevice = drawer.GraphicsDevice;
            Quad rect = drawer.DisplayedRectangle;

            // Configure default effect matrices
            _defaultEffect.World = SceneNode.Matrix.ToMatrix4X4(SceneNode.Depth);
            _defaultEffect.View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);
            _defaultEffect.Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, float.MinValue / 2, float.MaxValue / 2);

            drawer.SpriteBatchStack.Push(null);

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
                    if (part.VertexCount == 0)
                        continue;

                    Effect effect = part.Effect;
                    if (effect == null)
                    {
                        // If no effect provided, use default effect
                        effect = _defaultEffect;

                        // Enable texture if necessary
                        _defaultEffect.TextureEnabled = TextureSource?.Texture != null;//material.Textures.Count > 0;
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
                            currentPass = pass;
                        }

                        // Interesting links:
                        // http://www.shawnhargreaves.com/blog/spritebatch-billboards-in-a-3d-world.html
                        // https://github.com/MonoGame/MonoGame/blob/3e65abb158de2e07c72d0831dd971f594ff76a18/MonoGame.Framework/Graphics/Effect/SpriteEffect.cs#L71
                        // https://community.monogame.net/t/solved-drawing-primitives-and-spritebatch/10015/4

                        graphicsDevice.Textures[0] = TextureSource?.Texture;
                        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                        graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                        graphicsDevice.BlendState = BlendState.AlphaBlend;
                        graphicsDevice.DepthStencilState = DepthStencilState.None;

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
            if (vertexCount == 0)
                return 0;

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