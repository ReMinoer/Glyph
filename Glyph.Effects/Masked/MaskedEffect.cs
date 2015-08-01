using System;
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

        protected abstract void ApplyEffect(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);

        public override void Initialize()
        {
        }

        public override void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            Vector2 virtualSize = Resolution.Instance.VirtualSize;
            MaskRender = new RenderTarget2D(graphicsDevice, (int)virtualSize.X, (int)virtualSize.Y);

            _square = contentLibrary.GetTexture("square");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (!Enabled)
                return;

            graphicsDevice.SetRenderTarget(MaskRender);
            graphicsDevice.Clear(Color.Black);

            Vector2 virtualSize = Resolution.Instance.VirtualSize;
            var virtualRectangle = new Rectangle(0, 0, (int)virtualSize.X, (int)virtualSize.Y);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(_square, virtualRectangle, Color.White * (1 - Opacity));
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null,
                Camera.MatrixPosition * Camera.MatrixZoom);

            foreach (MaskPatch patch in Patches)
            {
                Texture2D patchSprite = PatchSprites[patch.PatchIndex];

                float scale = patch.Size / patchSprite.Width;
                Vector2 position = patch.Center - patchSprite.Size() / 2f * scale;

                spriteBatch.Draw(patchSprite, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }

        public override void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (!Enabled)
                return;

            ApplyEffect(spriteBatch, graphicsDevice);
        }

        public override void Dispose()
        {
            MaskRender.Dispose();
        }
    }
}