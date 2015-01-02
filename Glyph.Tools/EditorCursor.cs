using System;
using Glyph.Input;
using Glyph.Input.StandardActions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Tools
{
    static public class EditorCursor
    {
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
                    (int)((int)Position.X / (Resolution.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.X);
                _positionSpace.Y =
                    (int)((int)Position.Y / (Resolution.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.Y);
                return _positionSpace;
            }
        }
        static public Rectangle Selection
        {
            get
            {
                _selection.X = (int)Math.Min(PointB.X, PointA.X);
                _selection.Y = (int)Math.Min(PointB.Y, PointA.Y);
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
                    ((int)Math.Min(PointB.X, PointA.X) / (Resolution.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.X);
                _selectionSpace.Y =
                    (int)
                    ((int)Math.Min(PointB.Y, PointA.Y) / (Resolution.ScaleRatio * Camera.Zoom) + Camera.VectorPosition.Y);
                _selectionSpace.Width =
                    (int)((int)Math.Abs(PointB.X - PointA.X) / (Resolution.ScaleRatio * Camera.Zoom));
                _selectionSpace.Height =
                    (int)((int)Math.Abs(PointB.Y - PointA.Y) / (Resolution.ScaleRatio * Camera.Zoom));
                return _selectionSpace;
            }
        }
        static private readonly Vector2 OriginCursor = new Vector2(5, 5);

        static private Point _positionSpace;

        static private Rectangle _selection;

        static private Rectangle _selectionSpace;

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
            if (input.IsActionDownNow(DeveloperActions.StatusDisplay))
                Active = !Active;

            if (!Active)
                return;

            Position = new Vector2(input.Mouse.X, input.Mouse.Y);

            if (Position.X < 0)
                Position = Position.SetX(0);
            if (Position.X >= Resolution.Size.X - 1)
                Position = Position.SetX(Resolution.Size.X - 1);
            if (Position.Y < 0)
                Position = Position.SetY(0);
            if (Position.Y >= Resolution.Size.Y - 1)
                Position = Position.SetY(Resolution.Size.Y - 1);

            if (input.IsClicDownNow)
            {
                PointA = Position;
                PointB = Position;
            }
            else if (input.IsClicUpNow)
                PointB = Position;
            else if (input.Mouse.LeftButton == ButtonState.Pressed)
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