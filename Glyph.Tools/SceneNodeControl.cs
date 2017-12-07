using System;
using System.Linq;
using Diese.Collections;
using Diese.Injection;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools
{
    public class SceneNodeControl : GlyphObject
    {
        public float PositionHandleDistance = 100;
        private readonly SceneNode _sceneNode;
        private readonly PositionBinding _positionBinding;

        private SceneNode _selection;

        public SceneNode Selection
        {
            get => _selection;
            set
            {
                _selection = value;
                _positionBinding.Binding = _selection;
            }
        }

        public SceneNodeControl(GlyphInjectionContext context)
            : base(context)
        {
            _sceneNode = Add<SceneNode>();
            _positionBinding = Add<PositionBinding>();

            Add<Handle>();

            var xHandle = Add<Handle>();
            xHandle.FixedX = true;
            xHandle.LocalPosition = PositionHandleDistance * Vector2.UnitX;
            
            var yHandle = Add<Handle>();
            yHandle.FixedY = true;
            yHandle.LocalPosition = PositionHandleDistance * Vector2.UnitY;

            Schedulers.Draw.Plan(PrepareDraw).AtStart();
            Schedulers.Draw.Plan(FinalizeDraw).AtEnd();
        }

        private void PrepareDraw(IDrawer drawer)
        {
            var spriteBatchContext = new SpriteBatchContext
            {
                SpriteSortMode = SpriteSortMode.Immediate,
                TransformMatrix = drawer.ResolutionMatrix  * drawer.ViewMatrix
            };
            
            drawer.SpriteBatchStack.Push(spriteBatchContext);
        }

        private void FinalizeDraw(IDrawer drawer)
        {
            drawer.SpriteBatchStack.Pop();
        }

        public class Handle : GlyphObject
        {
            private readonly SceneNode _sceneNode;
            
            private readonly ReferentialCursorControl _virtualScreenCursor;
            private readonly ActivityControl _move;

            private readonly SceneNodeControl _sceneNodeControl;
            private bool _moving;
            private readonly FilledCircleSprite _filledCircleSprite;
            public bool FixedX { get; set; }
            public bool FixedY { get; set; }

            public Vector2 LocalPosition
            {
                get => _sceneNode.LocalPosition;
                set => _sceneNode.LocalPosition = value;
            }
            
            public Circle Circle => new Circle(_sceneNode.Position, _filledCircleSprite.Radius);

            public Handle(GlyphInjectionContext context, SceneNodeControl sceneNodeControl, InputClientManager inputClientManager)
                : base(context)
            {
                _sceneNodeControl = sceneNodeControl;
                
                _sceneNode = Add<SceneNode>();
                
                _filledCircleSprite = Add<FilledCircleSprite>();
                _filledCircleSprite.Radius = 10;
                
                var spriteTransformer = Add<SpriteTransformer>();
                
                if (FixedX)
                    spriteTransformer.Color = Color.Red;
                else if (FixedY)
                    spriteTransformer.Color = Color.Blue;
                
                Add<SpriteRenderer>();

                var controls = Add<Controls>();
                controls.Tags.Add(ControlLayerTag.Tools);
                controls.RegisterMany(new IControl[]
                {
                    _virtualScreenCursor = inputClientManager.CursorControls.VirtualScreenPosition,
                    _move = new ActivityControl("Move handle", InputSystem.Instance.Mouse[MouseButton.Left])
                });

                Schedulers.Update.Plan(HandleInput);
            }

            private void HandleInput(ElapsedTime elapsedTime)
            {
                //if (_move.IsActive(out InputActivity inputActivity))
                //{
                //    _virtualScreenCursor.IsActive(out System.Numerics.Vector2 virtualPosition);
                //    IView view = ViewManager.Main.GetViewAtPoint(virtualPosition.AsMonoGameVector(), null, out Vector2 viewPosition);
                    
                //    if (Circle.ContainsPoint(view.ViewToScene(viewPosition)) && inputActivity.IsTriggered())
                //    {
                //        _moving = true;
                //    }
                //    else if (inputActivity.IsPressed() && _moving)
                //    {
                //    }

                //    if (inputActivity.IsTriggered())
                //        _move.HandleInputs();
                //}
                //else
                //    _moving = false;
            }
        }
    }
}