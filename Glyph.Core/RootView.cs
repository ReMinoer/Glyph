using System;
using Glyph.Composition;
using Glyph.Core.Base;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Core
{
    public class RootView : ViewBase
    {
        private IDrawClient _drawClient;

        public IDrawClient DrawClient
        {
            get => _drawClient;
            set
            {
                if (_drawClient == value)
                    return;

                if (_drawClient != null)
                    _drawClient.SizeChanged -= DrawClientOnSizeChanged;

                _drawClient = value;
                DrawClientOnSizeChanged();

                if (_drawClient != null)
                    _drawClient.SizeChanged += DrawClientOnSizeChanged;

                void DrawClientOnSizeChanged(Vector2 obj = default)
                {
                    Refresh();
                    SizeChanged?.Invoke(this, Size);
                }
            }
        }

        public Vector2 Size => DrawClient?.Size ?? Vector2.Zero;
        protected override Quad Shape => new TopLeftRectangle(Vector2.Zero, Size);

        public override event EventHandler<Vector2> SizeChanged;

        public override void Draw(IDrawer drawer)
        {
            drawer.GraphicsDevice.Clear(Color.Black);

            if (!this.Displayed(drawer, drawer.Client))
                return;

            var spriteBatchContext = new SpriteBatchContext { SpriteSortMode = SpriteSortMode.BackToFront, TransformMatrix = RenderMatrix };
            drawer.SpriteBatchStack.Push(spriteBatchContext);
            drawer.Root?.Draw(drawer);
            drawer.SpriteBatchStack.Pop();
        }
    }
}