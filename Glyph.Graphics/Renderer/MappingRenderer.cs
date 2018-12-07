﻿using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public delegate bool RenderingBehaviour<TData>(TData caseData, MappingRenderer<TData> mappingRenderer);

    public class MappingRenderer<TData> : RendererBase
    {
        public ISpriteSheet SpriteSheet { get; }
        public IGrid<TData> Grid { get; set; }
        public Transformation Transformation { get; set; }
        public float Depth { get; set; }
        public RenderingBehaviour<TData> RenderingBehaviour { get; set; }
        protected override ISceneNode SceneNode { get; }

        protected override float DepthProtected => Depth;
        public override IArea Area => Grid.BoundingBox;

        public MappingRenderer(ISpriteSheet spriteSheet, SceneNode sceneNode)
            : base(spriteSheet)
        {
            SpriteSheet = spriteSheet;
            SceneNode = sceneNode;
            Transformation = Transformation.Identity;
        }

        protected override void Render(IDrawer drawer)
        {
            TopLeftRectangle cameraRectangle = drawer.DisplayedRectangle.BoundingBox;
            TopLeftRectangle drawnRectangle = Grid.BoundingBox;

            if (!drawnRectangle.Intersects(cameraRectangle, out TopLeftRectangle visibleRectangle))
                return;

            Rectangle visibleSubGrid = RectangleExtensions.ClampToRectangle(Grid.ToGridRange(visibleRectangle).ToFloats(), Grid.Bounds.ToFloats()).ToIntegers();

            for (int i = visibleSubGrid.Top; i < visibleSubGrid.Bottom; i++)
                for (int j = visibleSubGrid.Left; j < visibleSubGrid.Right; j++)
                {
                    if (!RenderingBehaviour(Grid[i, j], this))
                        continue;

                    Vector2 position = Grid.ToWorldPoint(i, j) + Grid.Delta / 2;

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, SpriteTransformer.Color,
                            Transformation.Rotation, SpriteTransformer.Origin, Transformation.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, Depth);
                    else
                    {
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, Color.White,
                            Transformation.Rotation, Source.GetDefaultOrigin(), Transformation.Scale, SpriteEffects.None, Depth);
                    }
                }
        }
    }
}