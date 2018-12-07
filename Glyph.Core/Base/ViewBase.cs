using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Base
{
    public abstract class ViewBase : GlyphContainer, IView
    {
        public bool Visible { get; set; } = true;
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
        public ICamera Camera { get; set; }
        public bool IsVoid => Shape.IsVoid;
        public TopLeftRectangle BoundingBox => Shape.BoundingBox;
        public Vector2 Center => Shape.Center;
        public Quad DisplayedRectangle => Camera.Transform(Shape);
        public Matrix RenderMatrix => Camera.RenderTransformation.Matrix * Matrix.CreateTranslation((Shape.Size / 2).ToVector3());

        protected abstract Quad Shape { get; }
        IArea IBoxedComponent.Area => Shape;
        Vector2 IView.Size => Shape.Size;

        public abstract event EventHandler<Vector2> SizeChanged;

        public bool IsVisibleOnView(Vector2 position)
        {
            return Visible && DisplayedRectangle.ContainsPoint(position);
        }

        public Vector2 Transform(Vector2 position) => position - Shape.Size / 2;
        public Vector2 InverseTransform(Vector2 position) => position + Shape.Size / 2;

        public Transformation Transform(Transformation transformation)
        {
            var result = new Transformation(transformation);
            transformation.Translation -= Shape.Size / 2;
            return result;
        }

        public Transformation InverseTransform(Transformation transformation)
        {
            var result = new Transformation(transformation);
            transformation.Translation += Shape.Size / 2;
            return result;
        }

        public bool ContainsPoint(Vector2 point) => Shape.ContainsPoint(point);
        public bool Intersects(Segment segment) => Shape.Intersects(segment);
        public bool Intersects(Circle circle) => Shape.Intersects(circle);

        public bool Intersects<T>(T edgedShape)
            where T : IEdgedShape => Shape.Intersects(edgedShape);

        public abstract void Draw(IDrawer drawer);
    }
}