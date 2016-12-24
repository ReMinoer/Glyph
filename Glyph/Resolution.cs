using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    // WATCH : Viewport improvement behaviour
    public class Resolution
    {
        private Vector2 _windowSize = new Vector2(800, 450);
        private Vector2 _size;
        private Matrix _scaleMatrix;
        private bool _dirtyMatrix = true;
        public float ScaleRatio => Size.X / VirtualResolution.Size.X;
        public Vector2 WindowMargin => (WindowSize - Size) / 2;

        public Vector2 Size
        {
            get { return _size; }
            private set
            {
                if (_size == value)
                    return;

                _size = value;
                SizeChanged?.Invoke(_size);
            }
        }

        public Vector2 WindowSize
        {
            get { return _windowSize; }
            set
            {
                _windowSize = value;

                RefreshSize();
                _dirtyMatrix = true;
            }
        }

        public Matrix TransformationMatrix
        {
            get
            {
                if (!_dirtyMatrix)
                    return _scaleMatrix;

                Vector2 virtualSize = VirtualResolution.Size;

                float scale = WindowSize.X / WindowSize.Y < VirtualResolution.AspectRatio
                    ? WindowSize.X / virtualSize.X
                    : WindowSize.Y / virtualSize.Y;

                _scaleMatrix = Matrix.CreateScale(scale, scale, 1f);
                _dirtyMatrix = false;

                return _scaleMatrix;
            }
        }

        public event Action<Vector2> SizeChanged;

        public Resolution()
        {
            RefreshSize();
            VirtualResolution.SizeChanged += OnVirtualResolutionChanged;
        }

        public void OnVirtualResolutionChanged(Vector2 size)
        {
            RefreshSize();
            _dirtyMatrix = true;
        }

        private void RefreshSize()
        {
            float virtualRatio = VirtualResolution.AspectRatio;

            Size = WindowSize.X / WindowSize.Y > virtualRatio
                ? WindowSize.SetX(WindowSize.Y * virtualRatio)
                : WindowSize.SetY(WindowSize.X / virtualRatio);
        }

        public Vector2 WindowToScreen(Point windowPoint)
        {
            return windowPoint.ToVector2() - WindowMargin;
        }

        public Vector2 WindowToVirtualScreen(Point windowPoint)
        {
            return (windowPoint.ToVector2() - WindowMargin) / ScaleRatio;
        }

        public Point ScreenToWindow(Vector2 screenPosition)
        {
            return (screenPosition + WindowMargin).ToPoint();
        }

        public Point VirtualScreenToWindow(Vector2 virtualScreenPosition)
        {
            return (virtualScreenPosition * ScaleRatio + WindowMargin).ToPoint();
        }
    }
}