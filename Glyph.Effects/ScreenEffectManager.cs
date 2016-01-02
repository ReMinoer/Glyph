using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public class ScreenEffectManager : EffectComposite
    {
        static private ScreenEffectManager _instance;
        private RenderTarget2D _renderTargetA;
        private RenderTarget2D _renderTargetB;
        private int _renderStep;

        static public ScreenEffectManager Instance
        {
            get { return _instance ?? (_instance = new ScreenEffectManager()); }
        }

        public RenderTarget2D Output
        {
            get { return _renderStep % 2 == 0 ? _renderTargetA : _renderTargetB; }
        }

        protected ScreenEffectManager()
        {
        }

        public override void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            base.LoadContent(contentLibrary, graphicsDevice);

            _renderTargetA = new RenderTarget2D(graphicsDevice, (int)Resolution.Instance.VirtualSize.X,
                (int)Resolution.Instance.VirtualSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _renderTargetB = new RenderTarget2D(graphicsDevice, (int)Resolution.Instance.VirtualSize.X,
                (int)Resolution.Instance.VirtualSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        public void CleanFirstRender(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_renderTargetA);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Pink, 0, 0);
        }

        public override void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            _renderStep = 0;
            foreach (IEffectComponent effect in GetAllComponentsInChildren<IEffectComponent>())
            {
                if (!effect.Enabled)
                    continue;

                RenderTarget2D input = _renderStep % 2 == 0 ? _renderTargetA : _renderTargetB;
                RenderTarget2D output = _renderStep % 2 == 0 ? _renderTargetB : _renderTargetA;

                graphicsDevice.SetRenderTarget(output);
                graphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Pink, 0, 0);

                spriteBatch.Begin(SpriteSortMode.Immediate);
                try
                {
                    effect.Apply(spriteBatch, graphicsDevice);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                spriteBatch.Draw(input, Vector2.Zero, Color.White);
                spriteBatch.End();

                _renderStep++;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _renderTargetA.Dispose();
            _renderTargetB.Dispose();
        }
    }
}