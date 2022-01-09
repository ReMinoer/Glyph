using System.Collections.Generic;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class RectangleEditor : MultiAnchoredEditorBase<IAnchoredRectangleController>
    {
        public const float Unit = 100;

        private readonly Anchor _topLeftAnchor;
        private readonly Anchor _topRightAnchor;
        private readonly Anchor _bottomLeftAnchor;
        private readonly Anchor _bottomRightAnchor;

        private readonly HandleSetter _leftHandleSetter;
        private readonly HandleSetter _topHandleSetter;
        private readonly HandleSetter _rightHandleSetter;
        private readonly HandleSetter _bottomHandleSetter;

        public RectangleEditor(GlyphResolveContext context)
            : base(context)
        {
            var v = new Vector2(Unit, Unit);
            Color color = Color.White * 0.25f;

            _topLeftAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle topLeftHandle = _topLeftAnchor.CreateHandle(new TopLeftRectangle(new Vector2(-Unit, -Unit), v).ToMesh(color));
                topLeftHandle.Axes = Axes.Both;
                topLeftHandle.AxesEditedFromOrigin = Axes.Both;
                
                AdvancedRectangleBorderPositionHandle leftHandle = _topLeftAnchor.CreateHandle();
                _leftHandleSetter = new HandleSetter(leftHandle);
                leftHandle.Axes = Axes.Horizontal;
                leftHandle.AxesEditedFromOrigin = Axes.Horizontal;
                
                AdvancedRectangleBorderPositionHandle topHandle = _topLeftAnchor.CreateHandle();
                _topHandleSetter = new HandleSetter(topHandle);
                topHandle.Axes = Axes.Vertical;
                topHandle.AxesEditedFromOrigin = Axes.Vertical;
            }
            AddEditor(_topLeftAnchor);

            _topRightAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle topRightHandle = _topRightAnchor.CreateHandle(new TopLeftRectangle(new Vector2(0, -Unit), v).ToMesh(color));
                topRightHandle.Axes = Axes.Both;
                topRightHandle.AxesEditedFromOrigin = Axes.Vertical;

                AdvancedRectangleBorderPositionHandle rightHandle = _topRightAnchor.CreateHandle();
                _rightHandleSetter = new HandleSetter(rightHandle);
                rightHandle.Axes = Axes.Horizontal;
            }
            AddEditor(_topRightAnchor);

            _bottomLeftAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle bottomLeftHandle = _bottomLeftAnchor.CreateHandle(new TopLeftRectangle(new Vector2(-Unit, 0), v).ToMesh(color));
                bottomLeftHandle.Axes = Axes.Both;
                bottomLeftHandle.AxesEditedFromOrigin = Axes.Horizontal;

                AdvancedRectangleBorderPositionHandle bottomHandle = _bottomLeftAnchor.CreateHandle();
                _bottomHandleSetter = new HandleSetter(bottomHandle);
                bottomHandle.Axes = Axes.Vertical;
            }
            AddEditor(_bottomLeftAnchor);

            _bottomRightAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle bottomRightHandle = _bottomRightAnchor.CreateHandle(new TopLeftRectangle(Vector2.Zero, v).ToMesh(color));
                bottomRightHandle.Axes = Axes.Both;
            }
            AddEditor(_bottomRightAnchor);

            Schedulers.Update.Plan(UpdateAnchorTransformations).AtStart();
        }

        private void UpdateAnchorTransformations(ElapsedTime elapsedTime)
        {
            _topLeftAnchor.AnchorTransformation = new Transformation(EditedObject.Rectangle.Position, 0, 1);
            _topRightAnchor.AnchorTransformation = new Transformation(EditedObject.Rectangle.P1, 0, 1);
            _bottomLeftAnchor.AnchorTransformation = new Transformation(EditedObject.Rectangle.P2, 0, 1);
            _bottomRightAnchor.AnchorTransformation = new Transformation(EditedObject.Rectangle.P3, 0, 1);

            _leftHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(-Unit, 0), new Vector2(Unit, EditedObject.Rectangle.Size.Y));
            _topHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, -Unit), new Vector2(EditedObject.Rectangle.Size.X, Unit));
            _rightHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, 0), new Vector2(Unit, EditedObject.Rectangle.Size.Y));
            _bottomHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, 0), new Vector2(EditedObject.Rectangle.Size.X, Unit));
        }

        public class Anchor : AnchoredEditorBase<IAnchoredRectangleController>
        {
            private readonly List<AdvancedRectangleBorderPositionHandle> _handles;
            public override IEnumerable<IHandle> Handles => _handles;
            
            public ITransformation AnchorTransformation
            {
                get => AnchoredSceneNode.AnchorTransformation;
                set => AnchoredSceneNode.AnchorTransformation = value;
            }

            public Anchor(GlyphResolveContext context)
                : base(context)
            {
                _handles = new List<AdvancedRectangleBorderPositionHandle>();
            }

            public AdvancedRectangleBorderPositionHandle CreateHandle()
            {
                var handle = Add<AdvancedRectangleBorderPositionHandle>();

                _handles.Add(handle);
                return handle;
            }

            public AdvancedRectangleBorderPositionHandle CreateHandle(TriangulableShapeMesh<TopLeftRectangle> rectangleMesh)
            {
                AdvancedRectangleBorderPositionHandle handle = CreateHandle();
                handle.Rectangle = rectangleMesh.Shape;
                handle.DefaultMeshes.Add(rectangleMesh);
                handle.DefaultMeshes.Add(rectangleMesh.Shape.ToOutlineMesh(Color.White));

                return handle;
            }

            protected override void AssignEditedObjectToHandles(IAnchoredRectangleController editedObject)
            {
                foreach (AdvancedRectangleBorderPositionHandle handle in _handles)
                    handle.EditedObject = editedObject;
            }
        }

        public class HandleSetter
        {
            private readonly AdvancedRectangleBorderPositionHandle _handle;
            private readonly TriangulableShapeMesh<TopLeftRectangle> _backgroundMesh;
            private readonly EdgedShapeOutlineMesh<TopLeftRectangle> _borderMesh;

            public TopLeftRectangle Rectangle
            {
                get => _handle.Rectangle;
                set
                {
                    _handle.Rectangle = value;
                    _backgroundMesh.Shape = value;
                    _borderMesh.Shape = value;
                }
            }

            public HandleSetter(AdvancedRectangleBorderPositionHandle handle)
            {
                _handle = handle;
                _backgroundMesh = new TriangulableShapeMesh<TopLeftRectangle>(Color.White * 0.25f, TopLeftRectangle.Void);
                _borderMesh = new EdgedShapeOutlineMesh<TopLeftRectangle>(Color.White, TopLeftRectangle.Void);

                _handle.DefaultMeshes.Add(_backgroundMesh);
                _handle.DefaultMeshes.Add(_borderMesh);
            }
        }
    }
}