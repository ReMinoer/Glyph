using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.ShapeRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Tools
{
    public delegate void GridBrushDelegate<TCase>(IWriteableGrid<TCase> grid, Point point);

    public class MapEditor<TCase> : GlyphContainer, ILoadContent, IUpdate, IDraw, IEnableable
    {
        private readonly Cursor _cursor;
        private readonly RectangleComponentRenderer _cursorRenderer;
        private readonly InputManager _inputManager;
        private IWriteableGrid<TCase> _grid;
        private PoorGrid<TCase> _lastStateGrid;
        private Vector2? _beginMousePosition;
        private bool _fillmode;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public GridBrushDelegate<TCase> LeftBrush { get; set; }
        public GridBrushDelegate<TCase> RightBrush { get; set; }

        public IWriteableGrid<TCase> Grid
        {
            get { return _grid; }
            set
            {
                _grid = value;
                _lastStateGrid = new PoorGrid<TCase>(Grid.BoundingBox, Grid.Dimension.Columns, Grid.Dimension.Rows);
            }
        }

        public MapEditor(InputManager inputManager, Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            _inputManager = inputManager;

            Visible = true;
               
            Components.Add(_cursor = new Cursor());
            Components.Add(_cursorRenderer = new RectangleComponentRenderer(_cursor, lazyGraphicsDevice));

            _cursorRenderer.Color = Color.White.SetOpacity(0.5f);
        }

        public override void Initialize()
        {
            _cursor.Initialize();
            _cursorRenderer.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _cursorRenderer.LoadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;
            
            var mousePosition = _inputManager.GetValue<Vector2>(MouseInputs.ScenePosition);
            if (!Grid.ContainsPoint(mousePosition))
            {
                _cursorRenderer.Visible = false;
                return;
            }
            _cursorRenderer.Visible = true;

            IRectangle activeRectangle = new OriginRectangle(Grid.ToWorldPoint(Grid.ToGridPoint(mousePosition)), Grid.Delta);
            HandleMouseButton(MouseInputs.LeftButtonInputs, LeftBrush, ref activeRectangle);
            HandleMouseButton(MouseInputs.RightButtonInputs, RightBrush, ref activeRectangle);
            
            _cursor.SceneNode.Position = activeRectangle.Center;
            _cursor.Shape = new OriginRectangle(Vector2.Zero, activeRectangle.Size);

            _cursorRenderer.Update(elapsedTime);
        }

        private void HandleMouseButton(MouseButtonInputs inputs, GridBrushDelegate<TCase> brush, ref IRectangle activeRectangle)
        {
            var mousePosition = _inputManager.GetValue<Vector2>(MouseInputs.ScenePosition);

            if (_inputManager[inputs.Triggered])
            {
                _beginMousePosition = mousePosition;
                _fillmode = _inputManager.InputStates.KeyboardState.IsKeyDown(Keys.LeftControl);
            }

            if (_inputManager[inputs.Pressed] && _beginMousePosition.HasValue && brush != null)
            {
                if (_fillmode)
                {
                    IRectangle rectangle = MathUtils.GetBoundingBox(_beginMousePosition.Value, mousePosition);

                    Point topLeftPoint = Grid.ToGridPoint(rectangle.Origin);
                    Point bottomLeftPoint = Grid.ToGridPoint(rectangle.Origin + rectangle.Size);

                    foreach (IGridCase<TCase> significantCase in _lastStateGrid.SignificantCases)
                        Grid[significantCase.Point] = _lastStateGrid[significantCase.Point];

                    _lastStateGrid.ClearSignificantCases();

                    for (int i = topLeftPoint.Y; i <= bottomLeftPoint.Y; i++)
                        for (int j = topLeftPoint.X; j <= bottomLeftPoint.X; j++)
                        {
                            _lastStateGrid[i, j] = Grid[i, j];
                            brush(Grid, new Point(j, i));
                        }

                    Vector2 origin = Grid.ToWorldPoint(topLeftPoint);
                    Vector2 size = Grid.ToWorldPoint(bottomLeftPoint) - origin + Grid.Delta;
                    activeRectangle = new OriginRectangle(origin, size);
                }
                else
                {
                    brush(Grid, Grid.ToGridPoint(mousePosition));
                }
            }

            if (_inputManager[inputs.Released])
            {
                _beginMousePosition = null;
                _lastStateGrid.ClearSignificantCases();
            }
        }

        public void Draw(IDrawer drawer)
        {
            if (!Enabled || !Visible)
                return;

            _cursorRenderer.Draw(drawer);
        }

        private sealed class Cursor : GlyphContainer, IShapedComponent<IRectangle>
        {
            public bool Enabled { get; set; }
            public SceneNode SceneNode { get; }
            public IRectangle Shape { get; internal set; }

            ISceneNode IShapedComponent.SceneNode
            {
                get { return SceneNode; }
            }

            IShape IShapedComponent.Shape
            {
                get { return Shape; }
            }

            public Cursor()
            {
                Enabled = true;

                Components.Add(SceneNode = new SceneNode());
                Shape = new OriginRectangle();
            }

            public override void Initialize()
            {
                SceneNode.Initialize();
            }
        }
    }
}