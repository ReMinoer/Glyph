using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
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

                const float r = 100;
                const float twoPi = MathHelper.TwoPi;
                const float pi = MathHelper.Pi;
                const float piOver2 = MathHelper.PiOver2;
                float tilt = MathHelper.ToRadians(26.7f);

                var scene = root.Add<GlyphObject>();
                scene.Add<SceneNode>().RootNode();
                scene.Add<PrimitiveRenderer>().PrimitiveProviders.Add(new PrimitiveCollection
                {
                    new EllipsePrimitive(Color.Yellow, Color.Red, Vector2.Zero, r / 3, r / 8, thickness: r / 20, rotation: tilt, angleStart: pi, angleSize: pi),
                    new EllipsePrimitive(Color.Purple, Color.White, Vector2.Zero, r / 4 - r / 10),
                    new EllipsePrimitive(Color.White, Vector2.Zero, r / 4, thickness: r / 20),
                    new CenteredRectangle(Vector2.Zero, r / 10, r / 20).ToPrimitive(Color.Green),
                    new EllipsePrimitive(Color.LightGray, Vector2.Zero, r / 4, thickness: r / 20, angleSize: pi),
                    new EllipsePrimitive(Color.Gray, Vector2.Zero, r / 4 - r / 20, thickness: r / 20, angleSize: piOver2),
                    new EllipsePrimitive(Color.Yellow, Color.Red, Vector2.Zero, r / 3, r / 8, thickness: r / 20, rotation: tilt, angleSize: pi),
                    new Triangle(Vector2.Zero, -Vector2.UnitX * r, -Vector2.UnitY * r).ToPrimitive(Color.Aqua * 0.5f),
                    new Circle(Vector2.Zero, r).ToOutlinePrimitive(Color.White),
                    new TopLeftRectangle(-Vector2.One.Normalized() * r, Vector2.One.Normalized() * 2 * r).ToOutlinePrimitive(Color.White),
                    new Triangle(new Vector2(r, 0), new Vector2(r * (float)System.Math.Cos(twoPi / 3), r * (float)System.Math.Sin(twoPi / 3)), new Vector2(r * (float)System.Math.Cos(2 * twoPi / 3), r * (float)System.Math.Sin(2 * twoPi / 3))).ToOutlinePrimitive(Color.White),
                    new EllipseOutlinePrimitive(Color.White, Vector2.Zero, r * 1.1f, angleSize: piOver2),
                    new EllipseOutlinePrimitive(Color.White, Vector2.Zero, r * 1.125f, angleSize: piOver2 - 0.025f),
                    new EllipseOutlinePrimitive(Color.White, Vector2.Zero, r * 1.15f, angleSize: piOver2 - 0.05f),
                    new EllipseOutlinePrimitive(Color.White, Vector2.Zero, r * 1.175f, angleSize: piOver2 - 0.075f),
                    new EllipseOutlinePrimitive(Color.White, Vector2.Zero, r * 1.2f, angleSize: piOver2 - 0.1f),
                    new EllipseOutlinePrimitive(Vector2.Zero, r * 2, r / 2, rotation: tilt) {Colors = new [] {Color.Red, Color.Yellow}},
                    new EllipseOutlinePrimitive(Vector2.UnitY * r / 10, r * 2, r / 2, rotation: tilt, angleSize: pi) {Colors = new [] {Color.Red, Color.Yellow}},
                    new Segment(Vector2.Zero, Vector2.UnitX * r).ToPrimitive(Color.Red),
                    new Segment(Vector2.Zero, Vector2.UnitY * r).ToPrimitive(Color.Blue),
                    new Segment(Vector2.Zero, Vector2.One.Normalized() * r).ToPrimitive(Color.Green),
                    new LinePrimitive(Color.Yellow, Vector2.UnitX * r, Vector2.One.Normalized() * r, Vector2.UnitY * r),
                });

                game.Run();
            }
        }
    }
}