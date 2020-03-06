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

                const float radius = 100;

                var scene = root.Add<GlyphObject>();
                scene.Add<SceneNode>().RootNode();
                scene.Add<PrimitiveRenderer>().Primitives.AddRange(new IPrimitive[]
                {
                    new LinePrimitive(Color.Red, Vector2.Zero, Vector2.UnitX * radius),
                    new LinePrimitive(Color.Blue, Vector2.Zero, Vector2.UnitY * radius),
                    new LinePrimitive(Color.Green, Vector2.Zero, Vector2.One.Normalized() * radius),
                    new LinePrimitive(Color.Yellow, Vector2.UnitX * radius, Vector2.One.Normalized() * radius, Vector2.UnitY * radius),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, radius),
                    new CircleOutlinePrimitive(Color.White, Vector2.Zero, radius * 1.1f, angleSize: MathHelper.PiOver2),
                    new EllipseOutlinePrimitive(Vector2.Zero, radius * 2, radius / 2, rotation: MathHelper.ToRadians(26.7f)) {Colors = new [] {Color.Red, Color.Yellow}},
                    new EllipseOutlinePrimitive(Vector2.UnitY * radius / 10, radius * 2, radius / 2, rotation: MathHelper.ToRadians(26.7f), angleSize: MathHelper.Pi) {Colors = new [] {Color.Red, Color.Yellow}}
                });

                game.Run();
            }
        }
    }
}