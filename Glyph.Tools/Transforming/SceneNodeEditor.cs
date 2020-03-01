using System;
using Glyph.Core;
using Glyph.Graphics.Renderer.Primitives;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class SceneNodeEditor : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
        static public readonly Vector2 Unit = new Vector2(100, 100);

        private readonly AnchoredSceneNode _anchoredSceneNode;

        private readonly AdvancedPositionHandle _positionHandle;
        private readonly AdvancedPositionHandle _horizontalHandle;
        private readonly AdvancedPositionHandle _verticalHandle;
        private readonly AdvancedScaleHandle _scaleHandle;
        private readonly AdvancedRotationHandle _rotationHandle;
        
        private IWritableSceneNodeComponent _editedObject;
        public IWritableSceneNodeComponent EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                _anchoredSceneNode.AnchorNode = value;

                _positionHandle.EditedObject = value;
                _horizontalHandle.EditedObject = value;
                _verticalHandle.EditedObject = value;
                _scaleHandle.EditedObject = value;
                _rotationHandle.EditedObject = value;
            }
        }
        
        object IIntegratedEditor.EditedObject => EditedObject;

        private IDrawClient _raycastClient;

        public IDrawClient RaycastClient
        {
            get => _raycastClient;
            set
            {
                _raycastClient = value;

                _positionHandle.RaycastClient = value;
                _horizontalHandle.RaycastClient = value;
                _verticalHandle.RaycastClient = value;
                _scaleHandle.RaycastClient = value;
                _rotationHandle.RaycastClient = value;
            }
        }

        public SceneNodeEditor(GlyphResolveContext context)
            : base(context)
        {
            _anchoredSceneNode = Add<AnchoredSceneNode>();
            _anchoredSceneNode.IgnoreRotation = true;
            _anchoredSceneNode.IgnoreScale = true;
            _anchoredSceneNode.ProjectionConfiguration = x => x.WithViewDepthMax(1);

            Add<UserInterface>();

            _scaleHandle = Add<AdvancedScaleHandle>();
            _scaleHandle.LocalPosition = Unit.Multiply(2, 2);
            _scaleHandle.Size = Unit.Multiply(1, 1); 
            _scaleHandle.Color = Color.Black;

            _horizontalHandle = Add<AdvancedPositionHandle>();
            _horizontalHandle.Visible = false;
            _horizontalHandle.LocalPosition = Unit.Multiply(2.5f, 0);
            _horizontalHandle.Size = Unit.Multiply(2, 1);
            _horizontalHandle.Axes = Axes.Horizontal;
            _horizontalHandle.Color = Color.Red;

            _verticalHandle = Add<AdvancedPositionHandle>();
            _verticalHandle.Visible = false;
            _verticalHandle.LocalPosition = Unit.Multiply(0, 2.5f);
            _verticalHandle.Size = Unit.Multiply(1, 2);
            _verticalHandle.Axes = Axes.Vertical;
            _verticalHandle.Color = Color.Blue;

            _positionHandle = Add<AdvancedPositionHandle>();
            _positionHandle.Visible = false;
            _positionHandle.LocalPosition = Unit.Multiply(1, 1);
            _positionHandle.Size = Unit.Multiply(3, 3);
            _positionHandle.Color = Color.White;

            _rotationHandle = Add<AdvancedRotationHandle>();
            _rotationHandle.LocalPosition = Unit.Multiply(2, 2);
            _rotationHandle.Size = Unit.Multiply(3, 3); 
            _rotationHandle.Color = Color.Green;

            Schedulers.Draw.Plan(_horizontalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_verticalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_scaleHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_positionHandle).After(_rotationHandle);
            
            var lineObject = Add<GlyphObject>();
            lineObject.Add<SceneNode>();

            var horizontalLine = lineObject.Add<LineRenderer>();
            horizontalLine.Vertices = new[] { Vector2.Zero, Unit.Multiply(3, 0) };
            horizontalLine.Color = Color.Red;

            var verticalLine = lineObject.Add<LineRenderer>();
            verticalLine.Vertices = new[] { Vector2.Zero, Unit.Multiply(0, 3) };
            verticalLine.Color = Color.Blue;
        }
    }
}