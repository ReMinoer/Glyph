using System.Collections.Generic;
using System.Linq;
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
    public class PrimitiveRenderer : RendererBase
    {
        [Populatable, ResolveTargets(ResolveTargets.Fraternal)]
        public List<IPrimitive> Primitives { get; } = new List<IPrimitive>();
        
        public override IArea Area => MathUtils.GetBoundingBox(Primitives.SelectMany(x => x.Vertices));
        protected override ISceneNode SceneNode { get; }
        protected override float DepthProtected => SceneNode.Depth;

        public PrimitiveRenderer(SceneNode sceneNode)
        {
            SceneNode = sceneNode;
        }

        protected override void Render(IDrawer drawer)
        {
            if (Primitives.Count == 0)
                return;

            int totalVertexCount = Primitives.Sum(x => x.Vertices.Count);
            if (totalVertexCount == 0)
                return;

            drawer.SpriteBatchStack.Push(null);

            var vertexArray = new VertexPositionColor[totalVertexCount];

            int startIndex = 0;
            foreach (IPrimitive primitive in Primitives)
            {
                primitive.CopyToArray(vertexArray, startIndex);
                startIndex += primitive.Vertices.Count;
            }
            
            var vertexBuffer = new VertexBuffer(drawer.GraphicsDevice, typeof(VertexPositionColor), totalVertexCount, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertexArray);
            drawer.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            Quad rect = drawer.DisplayedRectangle;
            var basicEffect = new BasicEffect(drawer.GraphicsDevice)
            {
                World = SceneNode.Matrix,
                View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, 0, float.MaxValue),
                VertexColorEnabled = true
            };

            startIndex = 0;
            foreach (IPrimitive primitive in Primitives)
            {
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    primitive.DrawPrimitives(drawer.GraphicsDevice, startIndex);
                }
                startIndex += primitive.Vertices.Count;
            }

            drawer.GraphicsDevice.SetVertexBuffer(null);
            drawer.SpriteBatchStack.Pop();
        }
    }
}