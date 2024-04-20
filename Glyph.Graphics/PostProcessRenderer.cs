using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyph.Composition;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Niddle.Attributes;

namespace Glyph.Graphics
{
    public class PostProcessRenderer : GlyphContainer, ISpriteSource, ILoadContent
    {
        private readonly SpriteTarget _spriteTargetA;
        private readonly SpriteTarget _spriteTargetB;
        private int _renderStep;
        private Vector2 _size;

        [Populatable, ResolveTargets(ResolveTargets.Fraternal | ResolveTargets.BrowseAllAncestors)]
        public List<IEffectRenderer> EffectRenderers { get; } = new List<IEffectRenderer>();

        public Texture2D Texture => _renderStep % 2 == 0
            ? _spriteTargetA.RenderTarget
            : _spriteTargetB.RenderTarget;

        public Vector2 Size
        {
            get => _size;
            set
            {
                _size = value;
                _spriteTargetA.Size = value;
                _spriteTargetB.Size = value;
            }
        }

        Rectangle? ISpriteSource.Rectangle => null;
        Vector2 ISpriteSource.GetDefaultOrigin() => Texture.Bounds.Size.ToVector2() / 2;
        Rectangle ISpriteSource.GetDrawnRectangle() => Texture.Bounds;

        public event Action<ISpriteSource> Loaded;

        public PostProcessRenderer(Func<GraphicsDevice> lazyGraphicsDevice)
        {
            _spriteTargetA = new SpriteTarget(lazyGraphicsDevice);
            _spriteTargetB = new SpriteTarget(lazyGraphicsDevice);
            Components.Add(_spriteTargetA);
            Components.Add(_spriteTargetB);
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            _spriteTargetA.LoadContent(contentLibrary);
            _spriteTargetB.LoadContent(contentLibrary);

            Loaded?.Invoke(this);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await _spriteTargetA.LoadContentAsync(contentLibrary);
            await _spriteTargetB.LoadContentAsync(contentLibrary);

            Loaded?.Invoke(this);
        }

        public void CleanFirstRender(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(_spriteTargetA.RenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }

        public void Render(IDrawer drawer)
        {
            _renderStep = 0;
            foreach (IEffectRenderer effectRenderer in EffectRenderers.Where(x => x.Visible))
            {
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

                effectRenderer.PrepareEffect(drawer);

                drawer.SpriteBatchStack.Push(new SpriteBatchContext
                {
                    SpriteSortMode = SpriteSortMode.Immediate
                });

                drawer.GraphicsDevice.SetRenderTarget(output);
                drawer.GraphicsDevice.Clear(Color.Transparent);

                effectRenderer.ApplyEffect(drawer);

                drawer.SpriteBatchStack.Current.Draw(input, Vector2.Zero, Color.White);
                drawer.SpriteBatchStack.Pop();

                _renderStep++;
            }
        }
    }
}