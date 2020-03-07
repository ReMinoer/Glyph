using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
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
                const float pi = MathHelper.Pi;
                const float piOver2 = MathHelper.PiOver2;
                float tilt = MathHelper.ToRadians(26.7f);

                var scene = root.Add<GlyphObject>();
                scene.Add<SceneNode>().RootNode();
                scene.Add<PrimitiveRenderer>().Primitives.AddRange(new IPrimitive[]
                {
                    new EllipsePrimitive(Color.Yellow, Color.Red, Vector2.Zero, r / 3, r / 8, thickness: r / 20, rotation: tilt, angleStart: pi, angleSize: pi),
                    new CirclePrimitive(Color.Purple, Color.White, Vector2.Zero, r / 4 - r / 10),
                    new CirclePrimitive(Color.White, Vector2.Zero, r / 4, thickness: r / 20),
                    new CirclePrimitive(Color.LightGray, Vector2.Zero, r / 4, thickness: r / 20, angleSize: pi),
                    new CirclePrimitive(Color.Gray, Vector2.Zero, r / 4 - r / 20, thickness: r / 20, angleSize: piOver2),
                    new EllipsePrimitive(Color.Yellow, Color.Red, Vector2.Zero, r / 3, r / 8, thickness: r / 20, rotation: tilt, angleSize: pi),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r * 1.1f, angleSize: piOver2),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r * 1.125f, angleSize: piOver2 - 0.025f),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r * 1.15f, angleSize: piOver2 - 0.05f),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r * 1.175f, angleSize: piOver2 - 0.075f),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, r * 1.2f, angleSize: piOver2 - 0.1f),
                    new EllipseOutlinePrimitive(Vector2.Zero, r * 2, r / 2, rotation: tilt) {Colors = new [] {Color.Red, Color.Yellow}},
                    new EllipseOutlinePrimitive(Vector2.UnitY * r / 10, r * 2, r / 2, rotation: tilt, angleSize: pi) {Colors = new [] {Color.Red, Color.Yellow}},
                    new LinePrimitive(Color.Red, Vector2.Zero, Vector2.UnitX * r),
                    new LinePrimitive(Color.Blue, Vector2.Zero, Vector2.UnitY * r),
                    new LinePrimitive(Color.Green, Vector2.Zero, Vector2.One.Normalized() * r),
                    new LinePrimitive(Color.Yellow, Vector2.UnitX * r, Vector2.One.Normalized() * r, Vector2.UnitY * r),
                });

                game.Run();
            }
        }
    }
}