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
        private ICamera _camera;

        public ICamera Camera
        {
            get => _camera;
            set
            {
                if (_camera == value)
                    return;

                if (_camera != null)
                    _camera.TransformationChanged -= OnCameraTransformationChanged;

                _camera = value;
                OnCameraTransformationChanged();
                
                if (_camera != null)
                    _camera.TransformationChanged += OnCameraTransformationChanged;

                void OnCameraTransformationChanged(object sender = null, EventArgs e = null) => RefreshComputedProperties();
            }
        }

        public bool Visible { get; set; } = true;
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
        public Matrix RenderMatrix { get; private set; } = Matrix.Identity;
        public Quad DisplayedRectangle { get; private set; }

        public bool IsVoid => Shape.IsVoid;
        public TopLeftRectangle BoundingBox => Shape.BoundingBox;
        public Vector2 Center => Shape.Center;

        protected abstract Quad Shape { get; }
        IArea IBoxedComponent.Area => Shape;
        Vector2 IView.Size => Shape.Size;

        public abstract event EventHandler<Vector2> SizeChanged;
        public event EventHandler TransformationChanged;

        protected virtual void RefreshTransformation()
        {
            RefreshComputedProperties();
            TransformationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshComputedProperties()
        {
            DisplayedRectangle = Camera?.Transform(Shape) ?? new Quad();
            RenderMatrix = (Camera?.RenderTransformation.Matrix ?? Matrix.Identity) * Matrix.CreateTranslation((Shape.Size / 2).ToVector3());
        }

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