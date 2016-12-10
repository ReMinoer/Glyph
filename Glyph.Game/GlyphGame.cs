using System;
using System.Windows.Forms;
using Diese.Injection;
using Diese.Modelization;
using Microsoft.Xna.Framework;

namespace Glyph.Game
{
    public class GlyphGame : Microsoft.Xna.Framework.Game
    {
        public GlyphEngine Engine { get; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public Resolution Resolution { get; }
        public virtual bool IsFocus => IsActive && Form.ActiveForm?.Handle == Window.Handle;

        public GlyphGame(IConfigurator<IDependencyRegistry> dependencyConfigurator = null)
        {
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            IsMouseVisible = false;

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Resolution = Resolution.Instance;
            Resolution.Init(GraphicsDeviceManager, Window);

            GraphicsDeviceManager.PreferMultiSampling = true;
            GraphicsDeviceManager.ApplyChanges();

            Engine = new GlyphEngine(Content, GraphicsDeviceManager, dependencyConfigurator);
            Engine.Stopped += OnEngineStopped;
        }

        protected override void Initialize()
        {
            Engine.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Engine.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.BeginUpdate(gameTime);

            base.Update(gameTime);

            Engine.HandleInput(IsFocus);

            if (!IsActive)
                return;

            Engine.Update();
        }

        protected override bool BeginDraw()
        {
            Engine.BeginDraw();
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Engine.Draw(GraphicsDeviceManager);
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            Engine.EndDraw();
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            Engine.Start();
        }

        protected override void EndRun()
        {
            Engine.Stop();
            base.EndRun();
        }

        public void OnEngineStopped()
        {
            Exit();
        }

        private void OnClientSizeChanged(object sender, EventArgs args)
        {
            Resolution.SetWindow(Window.ClientBounds.Width, Window.ClientBounds.Height, Resolution.Instance.FullScreen);
        }
    }
}