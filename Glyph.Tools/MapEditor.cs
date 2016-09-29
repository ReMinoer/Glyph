using System;
using Fingear;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Input;
using Glyph.Input.StandardControls;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Glyph.Tools.ShapeRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools
{
    public delegate void GridBrushDelegate<TCase>(IWriteableGrid<TCase> grid, Point point);

    public class MapEditor<TCase> : GlyphContainer, ILoadContent, IUpdate, IDraw, IEnableable
    {
        private readonly Cursor _cursor;
        private readonly RectangleComponentRenderer _cursorRenderer;
        private readonly ControlManager _controlManager;
        private IWriteableGrid<TCase> _grid;
        private PoorGrid<TCase> _lastStateGrid;
        private Vector2? _beginMousePosition;
        private bool _fillmode;
        private Vector2 _mousePosition;
        private IRectangle _activeRectangle;
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

        public MapEditor(ControlManager controlManager, Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            _controlManager = controlManager;

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

            MouseControls mouseControls;
            if (_controlManager.TryGetLayer(out mouseControls))
            {
                Fingear.Vector2 mouseFingear;
                if (mouseControls.ScenePosition.IsActive(out mouseFingear))
                {
                    _mousePosition = mouseFingear.AsMonoGameVector();

                    if (!Grid.ContainsPoint(_mousePosition))
                    {
                        _cursorRenderer.Visible = false;
                        return;
                    }
                    _cursorRenderer.Visible = true;

                    _activeRectangle = new OriginRectangle(Grid.ToWorldPoint(Grid.ToGridPoint(_mousePosition)), Grid.Delta);
                }

                HandleMouseButton(mouseControls.Left, LeftBrush, ref _activeRectangle);
                HandleMouseButton(mouseControls.Right, RightBrush, ref _activeRectangle);
            }

            if (_activeRectangle != null)
            {
                _cursor.SceneNode.Position = _activeRectangle.Center;
                _cursor.Shape = new OriginRectangle(Vector2.Zero, _activeRectangle.Size);
            }

            _cursorRenderer.Update(elapsedTime);
        }

        private void HandleMouseButton(IControl<InputActivity> control, GridBrushDelegate<TCase> brush, ref IRectangle activeRectangle)
        {
            InputActivity inputActivity;
            if (!control.IsActive(out inputActivity))
                return;

            if (inputActivity.IsTriggered())
            {
                _beginMousePosition = _mousePosition;

                EditorControls editorControls;
                _fillmode = _controlManager.TryGetLayer(out editorControls) && editorControls.ShiftPressed.IsActive();
            }

            if (inputActivity.IsPressed())
            {
                if (_fillmode)
                {
                    IRectangle rectangle = MathUtils.GetBoundingBox(_beginMousePosition.Value, _mousePosition);

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
                    brush(Grid, Grid.ToGridPoint(_mousePosition));
                }
            }

            if (inputActivity.IsReleased())
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