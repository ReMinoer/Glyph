using System;
using System.Collections.Generic;
using Glyph.Core;
using Glyph.Graphics.Renderer.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public delegate bool RenderingBehaviour<TData>(TData caseData, MappingRenderer<TData> mappingRenderer);

    public class MappingRenderer<TData> : SpriteRendererBase
    {
        public ISpriteSheet SpriteSheet { get; }
        public IGrid<TData> Grid { get; set; }
        public Transformation Transformation { get; set; }
        public float RenderDepth { get; set; }
        public RenderingBehaviour<TData> RenderingBehaviour { get; set; }
        protected override ISceneNode SceneNode { get; }

        protected override float RenderDepthOverride => RenderDepth;
        public override IArea Area => Grid.BoundingBox;

        public MappingRenderer(ISpriteSheet spriteSheet, ISceneNode sceneNode)
            : base(spriteSheet)
        {
            SpriteSheet = spriteSheet;
            SceneNode = sceneNode;
            Transformation = Transformation.Identity;

            SubscribeDepthChanged(sceneNode);
        }

        protected override void Render(IDrawer drawer)
        {
            TopLeftRectangle cameraRectangle = drawer.DisplayedRectangle.BoundingBox;
            TopLeftRectangle drawnRectangle = Grid.BoundingBox;

            if (!drawnRectangle.Intersects(cameraRectangle, out TopLeftRectangle visibleRectangle))
                return;

            IEnumerable<int[]> indexIntersection = Grid.IndexIntersection(visibleRectangle);
            foreach (int[] indexes in indexIntersection)
            {
                if (!RenderingBehaviour(Grid[indexes], this))
                    continue;

                Vector2 position = Grid.ToWorldPoint(indexes);

                if (SpriteTransformer != null)
                    drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, SpriteTransformer.Color,
                        SceneNode.Rotation + Transformation.Rotation, SpriteTransformer.Origin, SceneNode.Scale * Transformation.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, RenderDepth);
                else
                {
                    drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, Color.White,
                        SceneNode.Rotation + Transformation.Rotation, Source.GetDefaultOrigin(), SceneNode.Scale * Transformation.Scale, SpriteEffects.None, RenderDepth);
                }
            }
        }
    }
}