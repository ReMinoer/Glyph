using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer.Primitives
{
    public class LineRenderer : RendererBase
    {
        protected override ISceneNode SceneNode { get; }
        protected override float DepthProtected => SceneNode.Depth;

        public override IArea Area => MathUtils.GetBoundingBox(Vertices);

        public Vector2[] Vertices { get; set; }
        public Color Color { get; set; }

        public LineRenderer(SceneNode sceneNode)
        {
            SceneNode = sceneNode;
        }

        protected override void Render(IDrawer drawer)
        {
            drawer.SpriteBatchStack.Push(null);

            VertexPositionColor[] vertices = Vertices.Select(x => new VertexPositionColor(x.ToVector3(), Color)).ToArray();
            var vertexBuffer = new VertexBuffer(drawer.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            Quad rect = drawer.DisplayedRectangle;
            var basicEffect = new BasicEffect(drawer.GraphicsDevice)
            {
                World = SceneNode.Matrix,
                View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreateOrthographicOffCenter(rect.Left, rect.Right, rect.Bottom, rect.Top, 0, float.MaxValue),
                VertexColorEnabled = true
            };

            drawer.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                drawer.GraphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, vertices.Length - 1);
            }

            drawer.GraphicsDevice.SetVertexBuffer(null);
            drawer.SpriteBatchStack.Pop();
        }
    }
}