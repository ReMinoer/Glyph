using System.Linq;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics.Renderer.Primitives;
using Glyph.Tools;
using Microsoft.Xna.Framework;

namespace Glyph.Demos.PrimitiveRendering
{
    static public class Program
    {
        static public void Main()
        {
            using (var game = new GlyphGame())
            {
                GlyphEngine engine = game.Engine;
                GlyphObject root = engine.Root;

                root.Add<SceneNode>();
                engine.InteractionManager.Root.Add(root.Add<InteractiveRoot>().Interactive);

                var freeCamera = root.Add<FreeCamera>();
                freeCamera.View = engine.RootView;
                freeCamera.Client = game;

                const float unit = 100;

                var scene = root.Add<GlyphObject>();
                scene.Add<SceneNode>().RootNode();

                var horizontalLine = scene.Add<LineRenderer>();
                horizontalLine.Color = Color.Red;
                horizontalLine.Vertices = new[] { Vector2.Zero, Vector2.UnitX * unit };

                var verticalLine = scene.Add<LineRenderer>();
                verticalLine.Color = Color.Blue;
                verticalLine.Vertices = new[] { Vector2.Zero, Vector2.UnitY * unit };

                var diagonalLine = scene.Add<LineRenderer>();
                diagonalLine.Color = Color.Green;
                diagonalLine.Vertices = new[] { Vector2.Zero, Vector2.One.Normalized() * unit };
                
                var lineStrip = scene.Add<LineRenderer>();
                lineStrip.Color = Color.Yellow;
                lineStrip.Vertices = new[] { Vector2.UnitX * unit, Vector2.One.Normalized() * unit, Vector2.UnitY * unit };

                const int circleLineSampling = 64;
                var circleLine = scene.Add<LineRenderer>();
                circleLine.Color = Color.White;
                circleLine.Vertices = Enumerable.Range(0, circleLineSampling + 1)
                                                .Select(x => x * MathHelper.TwoPi / circleLineSampling)
                                                .Select(x => new Vector2((float)System.Math.Cos(x), (float)System.Math.Sin(x)) * unit).ToArray();

                game.Run();
            }
        }
    }
}