using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Graphics
{
    public class ViewEffectManager : SpriteSourceBase, ILoadContent, IUpdate
    {
        private readonly Func<GraphicsDevice> _lazyGraphicsDevice;
        private readonly SpriteTarget _spriteTargetA;
        private readonly SpriteTarget _spriteTargetB;
        private int _renderStep;
        private Vector2 _size;
        public List<IEffectComponent> Effects { get; private set; }
        public override event Action<ISpriteSource> Loaded;

        public override Texture2D Texture
        {
            get { return _renderStep % 2 == 0 ? _spriteTargetA.RenderTarget : _spriteTargetB.RenderTarget; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                _spriteTargetA.Size = value;
                _spriteTargetB.Size = value;
            }
        }

        public ViewEffectManager(Func<GraphicsDevice> lazyGraphicsDevice)
        {
            _lazyGraphicsDevice = lazyGraphicsDevice;

            _spriteTargetA = new SpriteTarget(lazyGraphicsDevice);
            _spriteTargetB = new SpriteTarget(lazyGraphicsDevice);

            Effects = new List<IEffectComponent>();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (IEffectComponent effectComponent in Effects)
                effectComponent.LoadContent(contentLibrary, _lazyGraphicsDevice());

            _spriteTargetA.LoadContent(contentLibrary);
            _spriteTargetB.LoadContent(contentLibrary);

            Loaded?.Invoke(this);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (IEffectComponent effectComponent in Effects)
                effectComponent.Update(elapsedTime.GameTime);
        }

        public void CleanFirstRender(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_spriteTargetA.RenderTarget);
            graphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Pink, 0, 0);
        }

        public void Prepare(IDrawer drawer)
        {
            foreach (IEffectComponent effectComponent in Effects)
                effectComponent.Prepare(drawer);
        }

        public void Apply(IDrawer drawer)
        {
            _renderStep = 0;
            foreach (IEffectComponent effect in Effects.SelectMany(x => x.ChildrenQueue()))
            {
                if (!effect.Enabled)
                    continue;

                RenderTarget2D input, output;
                if (_renderStep % 2 == 0)
                {
                    input = _spriteTargetA.RenderTarget;
                    output = _spriteTargetB.RenderTarget;
                }
                else
                {
                    input = _spriteTargetB.RenderTarget;
                    output = _spriteTargetA.RenderTarget;
                }

                drawer.GraphicsDevice.SetRenderTarget(output);
                drawer.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Pink, 0, 0);

                drawer.SpriteBatchStack.Current.Begin(SpriteSortMode.Immediate);
                try
                {
                    effect.Apply(drawer);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                drawer.SpriteBatchStack.Current.Draw(input, Vector2.Zero, Color.White);
                drawer.SpriteBatchStack.Current.End();

                _renderStep++;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (IEffectComponent effectComponent in Effects)
                effectComponent.Dispose();

            _spriteTargetA.Dispose();
            _spriteTargetB.Dispose();
        }
    }
}