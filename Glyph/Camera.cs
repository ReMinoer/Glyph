using System;
using Glyph.Transition;
using Microsoft.Xna.Framework;

namespace Glyph
{
    // Commentaire de test pour subtree 2
    public enum CameraMode
    {
        Auto,
        Script
    }

    static public class Camera
    {
        static public CameraMode Mode { get; set; }
        static public bool IsAdaptToEdges { get; set; }
        static public Vector2 Center
        {
            get { return _center; }
            set
            {
                if (_center == value)
                    return;

                _center = value;
                _dirtyMatrixPosition = true;
            }
        }
        static public Vector2 Destination { get; set; }
        static public float Speed { get; set; }
        static public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (Math.Abs(_zoom - value) < float.Epsilon)
                {
                    _zoom = value;
                    return;
                }

                _zoom = value;
                ZoomGoal = value;
                _dirtyMatrixPosition = true;
                _dirtyMatrixZoom = true;
            }
        }
        static public float ZoomGoal { get; set; }
        static public float ZoomSpeed { get; set; }
        static public Vector2 Relative
        {
            get { return _relative; }
            set
            {
                if (_relative == value)
                    return;

                _relative = value;
                RelativeGoal = value;
                _dirtyMatrixPosition = true;
            }
        }
        static public Vector2 RelativeGoal { get; set; }
        static public float RelativeSpeed { get; set; }
        static public Vector2 Size { get { return Resolution.VirtualSize; } }
        static public Rectangle DisplayRectangle
        {
            get
            {
                Rectangle displayRectangle;
                displayRectangle.X = (int)Center.X - (int)(Size.X / Zoom) / 2;
                displayRectangle.Y = (int)Center.Y - (int)(Size.Y / Zoom) / 2;
                displayRectangle.Width = (int)(Size.X / Zoom);
                displayRectangle.Height = (int)(Size.Y / Zoom);
                return displayRectangle;
            }
        }
        static public Vector2 PositionByDefault { get { return Center.Substract(Size.X / 2); } }
        static public Vector3 VectorPosition
        {
            get
            {
                Vector3 vectorPosition;
                vectorPosition.X = Center.X - (Size.X / Zoom) / 2;
                vectorPosition.Y = Center.Y - (Size.Y / Zoom) / 2;
                vectorPosition.Z = 0;
                return vectorPosition;
            }
        }
        static public Matrix MatrixPosition
        {
            get
            {
                if (!_dirtyMatrixPosition)
                    return _matrixPosition;

                _matrixPosition = Matrix.CreateTranslation(-VectorPosition);
                _dirtyMatrixPosition = false;
                return _matrixPosition;
            }
        }
        static public Matrix MatrixZoom
        {
            get
            {
                if (!_dirtyMatrixZoom)
                    return _matrixZoom;

                _matrixZoom = Matrix.CreateScale(Zoom, Zoom, 1f);
                _dirtyMatrixZoom = false;
                return _matrixZoom;
            }
        }
        static private Vector2 _center;
        static public readonly TransitionVector2 TransitionCenter = new TransitionVector2(BezierFunction.Ease);
        static private float _zoom;
        static private Vector2 _relative;
        static private bool _dirtyMatrixPosition = true;
        static private Matrix _matrixPosition;
        static private bool _dirtyMatrixZoom = true;
        static private Matrix _matrixZoom;

        static public void Initialize()
        {
            Mode = CameraMode.Auto;
            Center = Vector2.Zero;
            Destination = Vector2.Zero;
            Relative = Vector2.Zero;
            Zoom = 1f;
            Speed = 1f;
            IsAdaptToEdges = true;

            ZoomSpeed = 1f;
            RelativeSpeed = 1f;

            _dirtyMatrixPosition = true;
            _dirtyMatrixZoom = true;
        }

        static public void Update(GameTime gameTime, Vector2 center, Vector2 spaceSize)
        {
            Zoom = Zoom.Transition(ZoomGoal, ZoomSpeed, gameTime);
            Relative = Relative.Transition(RelativeGoal, RelativeSpeed, gameTime);

            switch (Mode)
            {
                case CameraMode.Auto:
                    if (IsAdaptToEdges && spaceSize != Vector2.Zero)
                        Center = CorrectionEdges(center + Relative, spaceSize);
                    else
                        Center = center + Relative;
                    break;

                case CameraMode.Script:
                    if (IsAdaptToEdges && spaceSize != Vector2.Zero)
                    {
                        Destination = CorrectionEdges(Destination, spaceSize);
                        Center = CorrectionEdges(Center, spaceSize);
                    }

                    if (Center != Destination)
                        Center = TransitionCenter.UpdateBySpeed(gameTime, Center, Destination, Speed);

                    break;
            }
        }

        static public Vector2 CorrectionEdges(Vector2 pos, Vector2 spaceSize)
        {
            Vector2 result = pos;

            if (result.X + Size.X / 2 / Zoom >= spaceSize.X)
                result = new Vector2(spaceSize.X - (Size.X / Zoom / 2), result.Y);
            if (result.X - Size.X / 2 / Zoom <= 0)
                result = new Vector2(Size.X / 2 / Zoom, result.Y);

            if (result.Y + Size.Y / 2 / Zoom >= spaceSize.Y)
                result = new Vector2(result.X, spaceSize.Y - (Size.Y / Zoom / 2));
            if (result.Y - Size.Y / Zoom / 2 <= 0)
                result = new Vector2(result.X, Size.Y / Zoom / 2);

            return result;
        }

        static public Vector2 PositionObjectOnScreen(Vector2 posVirtual)
        {
            return posVirtual - VectorPosition.XY() * (Resolution.ScaleRatio * Zoom);
        }
    }
}