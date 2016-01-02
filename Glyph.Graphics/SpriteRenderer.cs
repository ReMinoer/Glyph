﻿using Glyph.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteRenderer : Renderer
    {
        public SpriteRenderer(ISpriteSource source, SceneNode sceneNode)
            : base(source, sceneNode)
        {
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            if (SpriteTransformer != null)
                spriteBatch.Draw(Source.Texture, SceneNode.Position, Source.Rectangle, SpriteTransformer.Color,
                    SceneNode.Rotation, SpriteTransformer.Origin, SceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, SceneNode.Depth);
            else
            {
                spriteBatch.Draw(Source.Texture, SceneNode.Position, Source.Rectangle, Color.White,
                    SceneNode.Rotation, Source.GetDrawnRectangle().Center.ToVector2(), SceneNode.Scale, SpriteEffects.None, SceneNode.Depth);
            }
        }
    }
}