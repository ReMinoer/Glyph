using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects.Masked
{
    public abstract class MaskedEffect : EffectComponent
    {
        protected Texture2D[] PatchSprites;
        protected RenderTarget2D MaskRender;
        private Texture2D _square;
        public float Opacity { get; set; }
        public List<MaskPatch> Patches { get; set; }

        protected MaskedEffect()
        {
            Patches = new List<MaskPatch>();
        }

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            Vector2 virtualSize = VirtualResolution.Size;
            MaskRender = new RenderTarget2D(graphicsDevice, (int)virtualSize.X, (int)virtualSize.Y);

            _square = contentLibrary.GetTexture("square");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Prepare(IDrawer drawer)
        {
            if (!Enabled)
                return;

            drawer.GraphicsDevice.SetRenderTarget(MaskRender);
            drawer.GraphicsDevice.Clear(Color.Black);

            Vector2 virtualSize = VirtualResolution.Size;
            var virtualRectangle = new Rectangle(0, 0, (int)virtualSize.X, (int)virtualSize.Y);

            drawer.SpriteBatchStack.Current.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            drawer.SpriteBatchStack.Current.Draw(_square, virtualRectangle, Color.White * (1 - Opacity));
            drawer.SpriteBatchStack.Current.End();

            drawer.SpriteBatchStack.Current.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, drawer.ViewMatrix);

            foreach (MaskPatch patch in Patches)
            {
                Texture2D patchSprite = PatchSprites[patch.PatchIndex];

                float scale = patch.Size / patchSprite.Width;
                Vector2 position = patch.Center - patchSprite.Size() / 2f * scale;

                drawer.SpriteBatchStack.Current.Draw(patchSprite, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            drawer.SpriteBatchStack.Current.End();

            drawer.GraphicsDevice.SetRenderTarget(drawer.DefaultRenderTarget);
        }

        public override void Apply(IDrawer drawer)
        {
            if (!Enabled)
                return;

            ApplyEffect(drawer);
        }

        public override void Dispose()
        {
            MaskRender.Dispose();
        }

        protected abstract void ApplyEffect(IDrawer drawer);
    }
}