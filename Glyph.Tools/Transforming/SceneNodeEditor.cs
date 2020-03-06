using System;
using System.Collections.Generic;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class SceneNodeEditor : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
        static public readonly float Unit = 100;

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
            _scaleHandle.Visible = false;
            _scaleHandle.LocalPosition = new Vector2(2, 2) * Unit;
            _scaleHandle.Size = new Vector2(1, 1) * Unit; 
            _scaleHandle.Color = Color.Black;

            _horizontalHandle = Add<AdvancedPositionHandle>();
            _horizontalHandle.Visible = false;
            _horizontalHandle.LocalPosition = new Vector2(2.5f, 0) * Unit;
            _horizontalHandle.Size = new Vector2(2, 1) * Unit;
            _horizontalHandle.Axes = Axes.Horizontal;
            _horizontalHandle.Color = Color.Red;

            _verticalHandle = Add<AdvancedPositionHandle>();
            _verticalHandle.Visible = false;
            _verticalHandle.LocalPosition = new Vector2(0, 2.5f) * Unit;
            _verticalHandle.Size = new Vector2(1, 2) * Unit;
            _verticalHandle.Axes = Axes.Vertical;
            _verticalHandle.Color = Color.Blue;

            _positionHandle = Add<AdvancedPositionHandle>();
            _positionHandle.Visible = false;
            _positionHandle.LocalPosition = new Vector2(1, 1) * Unit;
            _positionHandle.Size = new Vector2(3, 3) * Unit;
            _positionHandle.Color = Color.White;

            _rotationHandle = Add<AdvancedRotationHandle>();
            _rotationHandle.Visible = false;
            _rotationHandle.LocalPosition = new Vector2(2, 2) * Unit;
            _rotationHandle.Size = new Vector2(3, 3) * Unit; 
            _rotationHandle.Color = Color.Green;

            Schedulers.Draw.Plan(_horizontalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_verticalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_scaleHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_positionHandle).After(_rotationHandle);
            
            var lineObject = Add<GlyphObject>();
            lineObject.Add<SceneNode>();
            lineObject.Add<PrimitiveRenderer>().Primitives.AddRange(new IPrimitive[]
            {
                new LinePrimitive(Color.Red, Vector2.Zero, new Vector2(3, 0) * Unit),
                new LinePrimitive(Color.Blue, Vector2.Zero, new Vector2(0, 3) * Unit),
                new LinePrimitive(Color.Black, new Vector2(1.5f, 2) * Unit, new Vector2(2, 2) * Unit, new Vector2(2, 1.5f) * Unit),
                new CircleOutlinePrimitive(Color.Green, Vector2.Zero, (new Vector2(2.5f, 2.5f) * Unit).Length(), MathHelper.ToRadians(15), MathHelper.ToRadians(60))
            });
        }
    }
}