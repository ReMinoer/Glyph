using Glyph.Core;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools
{
    public class SceneNodeEditor : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
        public const float PositionHandleDelta = 100;

        private readonly AnchoredSceneNode _anchoredSceneNode;

        private readonly PositionHandle _centralHandle;
        private readonly PositionHandle _horizontalHandle;
        private readonly PositionHandle _verticalHandle;
        
        private IWritableSceneNodeComponent _editedObject;
        public IWritableSceneNodeComponent EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                _anchoredSceneNode.AnchorNode = value;

                _centralHandle.EditedObject = value;
                _horizontalHandle.EditedObject = value;
                _verticalHandle.EditedObject = value;
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

                _centralHandle.RaycastClient = value;
                _horizontalHandle.RaycastClient = value;
                _verticalHandle.RaycastClient = value;
            }
        }

        public SceneNodeEditor(GlyphResolveContext context)
            : base(context)
        {
            _anchoredSceneNode = Add<AnchoredSceneNode>();
            _anchoredSceneNode.ProjectionConfiguration = x => x.WithViewDepthMax(1);

            Add<UserInterface>();

            _centralHandle = Add<PositionHandle>();

            _horizontalHandle = Add<PositionHandle>();
            _horizontalHandle.LocalPosition = PositionHandleDelta * Vector2.UnitX;
            _horizontalHandle.Color = Color.Red;
            _horizontalHandle.Axes = Axes.Horizontal;

            _verticalHandle = Add<PositionHandle>();
            _verticalHandle.LocalPosition = -PositionHandleDelta * Vector2.UnitY;
            _verticalHandle.Color = Color.Blue;
            _verticalHandle.Axes = Axes.Vertical;
        }
    }
}