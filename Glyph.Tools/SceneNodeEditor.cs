using System.Linq;
using Diese.Collections;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class SceneNodeEditor : GlyphObject, IIntegratedEditor<IWritableSceneNode>
    {
        public const float PositionHandleSize = 100;

        private readonly SceneNode _sceneNode;
        private readonly PositionBinding _positionBinding;

        private IWritableSceneNode _editedObject;
        public IWritableSceneNode EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                _positionBinding.Binding = value;
            }
        }
        
        object IIntegratedEditor.EditedObject => EditedObject;

        public SceneNodeEditor(GlyphInjectionContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            _positionBinding = Add<PositionBinding>();

            Add<Handle>();

            var horizontalHandle = Add<Handle>();
            horizontalHandle.LocalPosition = PositionHandleSize * Vector2.UnitX;
            horizontalHandle.Color = Color.Red;
            horizontalHandle.Axes = Axes.Horizontal;

            var verticalHandle = Add<Handle>();
            verticalHandle.LocalPosition = -PositionHandleSize * Vector2.UnitY;
            verticalHandle.Color = Color.Blue;
            verticalHandle.Axes = Axes.Vertical;
        }

        public class Handle : GlyphObject
        {
            private readonly SceneNodeEditor _sceneNodeEditor;
            private readonly ProjectionManager _projectionManager;
            private readonly SceneNode _sceneNode;
            private readonly FilledCircleSprite _filledCircleSprite;
            private readonly SpriteTransformer _spriteTransformer;
            
            private readonly ProjectionCursorControl _projectedCursor;
            private readonly ActivityControl _grab;

            private bool _grabbed;

            public Axes Axes { get; set; } = Axes.Both;

            public Vector2 LocalPosition
            {
                get => _sceneNode.LocalPosition;
                set => _sceneNode.LocalPosition = value;
            }

            public Color Color
            {
                get => _spriteTransformer.Color;
                set => _spriteTransformer.Color = value;
            }

            private Circle Circle => new Circle(_sceneNode.Position, _filledCircleSprite.Radius);

            public Handle(GlyphInjectionContext context, SceneNodeEditor sceneNodeEditor, RootView rootView, ProjectionManager projectionManager)
                : base(context)
            {
                _sceneNodeEditor = sceneNodeEditor;
                _projectionManager = projectionManager;

                _sceneNode = Add<SceneNode>();

                _filledCircleSprite = Add<FilledCircleSprite>();
                _filledCircleSprite.Radius = 10;

                _spriteTransformer = Add<SpriteTransformer>();
                Add<SpriteRenderer>();

                var controls = Add<Controls>();
                controls.Tags.Add(ControlLayerTag.Tools);
                controls.RegisterMany(new IControl[]
                {
                    _projectedCursor = new ProjectionCursorControl("Virtual cursor", InputSystem.Instance.Mouse.Cursor, rootView, _sceneNode, projectionManager),
                    _grab = new ActivityControl("Grab handle", InputSystem.Instance.Mouse[MouseButton.Left])
                });

                Schedulers.Update.Plan(HandleInput);
            }

            private void HandleInput(ElapsedTime elapsedTime)
            {
                if (_grab.IsActive(out InputActivity grabActivity) && (_grabbed || grabActivity.IsTriggered()))
                {
                    _projectedCursor.IsActive(out Vector2 projectedCursorPosition);

                    if (grabActivity.IsTriggered())
                    {
                        if (Circle.ContainsPoint(projectedCursorPosition))
                            _grabbed = true;
                    }

                    if (_grabbed)
                    {
                        Vector2 projectedObjectPosition = _projectionManager.ProjectFromPosition(_sceneNode, projectedCursorPosition)
                                                                            .To(_sceneNodeEditor.EditedObject)
                                                                            .ByRaycast()
                                                                            .First()
                                                                            .Value;

                        _sceneNodeEditor.EditedObject.Position = projectedObjectPosition;
                        _sceneNodeEditor._sceneNode.Position = projectedCursorPosition;
                    }
                }
                else
                {
                    _grabbed = false;
                }
            }
        }
    }
}