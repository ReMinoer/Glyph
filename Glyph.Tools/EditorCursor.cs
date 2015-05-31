using System;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools
{
    static public class EditorCursor
    {
        static private readonly Vector2 OriginCursor = new Vector2(5, 5);
        static private Point _positionSpace;
        static private Rectangle _selection;
        static private Rectangle _selectionSpace;
        static public bool Active { get; set; }
        static public Vector2 Position { get; set; }
        static public Vector2 PointA { get; set; }
        static public Vector2 PointB { get; set; }
        static private Texture2D TextureCross { get; set; }
        static private Texture2D TextureSelection { get; set; }

        static public Point PositionSpace
        {
            get
            {
                _positionSpace.X =
                    (int)((int)Position.X / (Resolution.Instance.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.X);
                _positionSpace.Y =
                    (int)((int)Position.Y / (Resolution.Instance.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.Y);
                return _positionSpace;
            }
        }

        static public Rectangle Selection
        {
            get
            {
                _selection.X = (int)MathHelper.Min(PointB.X, PointA.X);
                _selection.Y = (int)MathHelper.Min(PointB.Y, PointA.Y);
                _selection.Width = (int)Math.Abs(PointB.X - PointA.X);
                _selection.Height = (int)Math.Abs(PointB.Y - PointA.Y);
                return _selection;
            }
        }

        static public Rectangle SelectionSpace
        {
            get
            {
                _selectionSpace.X =
                    (int)
                        ((int)MathHelper.Min(PointB.X, PointA.X) / (Resolution.Instance.ScaleRatio * Camera.Zoom) +
                         Camera.VectorPosition.X);
                _selectionSpace.Y =
                    (int)
                        ((int)MathHelper.Min(PointB.Y, PointA.Y) / (Resolution.Instance.ScaleRatio * Camera.Zoom) +
                         Camera.VectorPosition.Y);
                _selectionSpace.Width =
                    (int)((int)Math.Abs(PointB.X - PointA.X) / (Resolution.Instance.ScaleRatio * Camera.Zoom));
                _selectionSpace.Height =
                    (int)((int)Math.Abs(PointB.Y - PointA.Y) / (Resolution.Instance.ScaleRatio * Camera.Zoom));
                return _selectionSpace;
            }
        }

        static public void Initialize()
        {
            Active = true;
            Position = Vector2.Zero;
            PointA = Vector2.Zero;
            PointB = Vector2.Zero;
        }

        static public void LoadContent(ContentLibrary ressources)
        {
            TextureCross = ressources.GetTexture("editor-cursor");
            TextureSelection = ressources.GetTexture("square");
        }

        static public void HandleInput(InputManager input)
        {
            if (input[DeveloperInputs.StatusDisplay])
                Active = !Active;

            if (!Active)
                return;

            Position = input.GetValue<Vector2>(MouseInputs.WindowPosition);

            if (Position.X < Resolution.Instance.WindowMargin.X)
                Position = Position.SetX(Resolution.Instance.WindowMargin.X);
            if (Position.X >= Resolution.Instance.Size.X + Resolution.Instance.WindowMargin.X - 1)
                Position = Position.SetX(Resolution.Instance.Size.X + Resolution.Instance.WindowMargin.X - 1);
            if (Position.Y < Resolution.Instance.WindowMargin.Y)
                Position = Position.SetY(Resolution.Instance.WindowMargin.Y);
            if (Position.Y >= Resolution.Instance.Size.Y + Resolution.Instance.WindowMargin.Y - 1)
                Position = Position.SetY(Resolution.Instance.Size.Y + Resolution.Instance.WindowMargin.Y - 1);

            if (input[MouseInputs.Left])
            {
                PointA = Position;
                PointB = Position;
            }
            else if (input[MouseInputs.LeftReleased])
                PointB = Position;
            else if (input[MouseInputs.LeftPressed])
                PointB = Position;
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            spriteBatch.Draw(TextureCross, Position - OriginCursor, null, Color.White);
            spriteBatch.Draw(TextureSelection, Selection, Color.Blue * 0.5f);
        }
    }
}