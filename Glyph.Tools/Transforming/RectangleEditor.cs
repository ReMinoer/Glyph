using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class RectangleEditor : MultiAnchoredEditorBase<IAnchoredRectangleController>, IHandle<IAnchoredRectangleController>
    {
        private readonly ProjectionManager _projectionManager;
        public const float Unit = 25;

        private readonly Anchor _topLeftAnchor;
        private readonly Anchor _topRightAnchor;
        private readonly Anchor _bottomLeftAnchor;
        private readonly Anchor _bottomRightAnchor;

        private readonly HandleSetter _positionHandleSetter;
        private readonly HandleSetter _leftHandleSetter;
        private readonly HandleSetter _topHandleSetter;
        private readonly HandleSetter _rightHandleSetter;
        private readonly HandleSetter _bottomHandleSetter;
        private readonly HandleSetter _topLeftHandleSetter;
        private readonly HandleSetter _topRightHandleSetter;
        private readonly HandleSetter _bottomLeftHandleSetter;
        private readonly HandleSetter _bottomRightHandleSetter;

        public Func<Vector2, Vector2> Revaluation
        {
            get => _topLeftAnchor.Revaluation;
            set
            {
                _topLeftAnchor.Revaluation = value;
                _topRightAnchor.Revaluation = value;
                _bottomLeftAnchor.Revaluation = value;
                _bottomRightAnchor.Revaluation = value;
            }
        }

        public ISceneNode ScaleAnchorNode { get; set; }

        public event EventHandler Grabbed;
        public event EventHandler Dragging;
        public event EventHandler Released;
        public event EventHandler Cancelled;

        public RectangleEditor(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context)
        {
            _projectionManager = projectionManager;

            _topLeftAnchor = Add<Anchor>();
            {
                AdvancedRectanglePositionHandle positionHandle = _topLeftAnchor.CreatePositionHandle();
                _positionHandleSetter = CreateHandleSetter(positionHandle);
                positionHandle.KeyboardEnabled = true;

                AdvancedRectangleBorderPositionHandle topLeftHandle = _topLeftAnchor.CreateBorderHandle();
                _topLeftHandleSetter = CreateHandleSetter(topLeftHandle);
                topLeftHandle.Axes = Axes.Both;
                topLeftHandle.AxesEditedFromOrigin = Axes.Both;
                
                AdvancedRectangleBorderPositionHandle leftHandle = _topLeftAnchor.CreateBorderHandle();
                _leftHandleSetter = CreateHandleSetter(leftHandle);
                leftHandle.Axes = Axes.Horizontal;
                leftHandle.AxesEditedFromOrigin = Axes.Horizontal;
                
                AdvancedRectangleBorderPositionHandle topHandle = _topLeftAnchor.CreateBorderHandle();
                _topHandleSetter = CreateHandleSetter(topHandle);
                topHandle.Axes = Axes.Vertical;
                topHandle.AxesEditedFromOrigin = Axes.Vertical;
            }
            AddEditor(_topLeftAnchor);

            _topRightAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle topRightHandle = _topRightAnchor.CreateBorderHandle();
                _topRightHandleSetter = CreateHandleSetter(topRightHandle);
                topRightHandle.Axes = Axes.Both;
                topRightHandle.AxesEditedFromOrigin = Axes.Vertical;

                AdvancedRectangleBorderPositionHandle rightHandle = _topRightAnchor.CreateBorderHandle();
                _rightHandleSetter = CreateHandleSetter(rightHandle);
                rightHandle.Axes = Axes.Horizontal;
            }
            AddEditor(_topRightAnchor);

            _bottomLeftAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle bottomLeftHandle = _bottomLeftAnchor.CreateBorderHandle();
                _bottomLeftHandleSetter = CreateHandleSetter(bottomLeftHandle);
                bottomLeftHandle.Axes = Axes.Both;
                bottomLeftHandle.AxesEditedFromOrigin = Axes.Horizontal;

                AdvancedRectangleBorderPositionHandle bottomHandle = _bottomLeftAnchor.CreateBorderHandle();
                _bottomHandleSetter = CreateHandleSetter(bottomHandle);
                bottomHandle.Axes = Axes.Vertical;
            }
            AddEditor(_bottomLeftAnchor);

            _bottomRightAnchor = Add<Anchor>();
            {
                AdvancedRectangleBorderPositionHandle bottomRightHandle = _bottomRightAnchor.CreateBorderHandle();
                _bottomRightHandleSetter = CreateHandleSetter(bottomRightHandle);
                bottomRightHandle.Axes = Axes.Both;
            }
            AddEditor(_bottomRightAnchor);

            SubscribeHandles();

            Schedulers.Update.Plan(UpdateAnchorTransformations).AtStart();
        }

        public override void Dispose()
        {
            UnsubscribeHandles();
            base.Dispose();
        }

        private HandleSetter CreateHandleSetter(AdvancedHandleBase<IAnchoredRectangleController> handle)
        {
            return new HandleSetter(handle);
        }

        private void SubscribeHandles()
        {
            foreach (IHandle handle in Handles)
            {
                handle.Grabbed += OnGrabbed;
                handle.Dragging += OnDragging;
                handle.Released += OnReleased;
                handle.Cancelled += OnCancelled;
            }
        }

        private void UnsubscribeHandles()
        {
            foreach (IHandle handle in Handles)
            {
                handle.Cancelled -= OnCancelled;
                handle.Released -= OnReleased;
                handle.Dragging -= OnDragging;
                handle.Grabbed -= OnGrabbed;
            }
        }

        private void OnGrabbed(object sender, EventArgs e) => Grabbed?.Invoke(this, EventArgs.Empty);
        private void OnDragging(object sender, EventArgs e) => Dragging?.Invoke(this, EventArgs.Empty);
        private void OnReleased(object sender, EventArgs e) => Released?.Invoke(this, EventArgs.Empty);
        private void OnCancelled(object sender, EventArgs e) => Cancelled?.Invoke(this, EventArgs.Empty);

        private void UpdateAnchorTransformations(ElapsedTime elapsedTime)
        {
            ITransformation transformation = EditedObject.IsLocalRectangle
                ? EditedObject.Anchor.LocalTransformation.InverseTransform(EditedObject.Anchor.Transformation)
                : Transformation.Identity;

            TopLeftRectangle liveRectangle = new TopLeftRectangle(EditedObject.LiveRectanglePosition, EditedObject.LiveRectangleSize);

            _topLeftAnchor.AnchorTransformation = transformation.Transform(new Transformation(liveRectangle.Position, 0, 1));
            _topRightAnchor.AnchorTransformation = transformation.Transform(new Transformation(liveRectangle.P1, 0, 1));
            _bottomLeftAnchor.AnchorTransformation = transformation.Transform(new Transformation(liveRectangle.P2, 0, 1));
            _bottomRightAnchor.AnchorTransformation = transformation.Transform(new Transformation(liveRectangle.P3, 0, 1));

            float? scale = null;
            if (ScaleAnchorNode != null)
            {
                SceneNode sceneNode = this.GetSceneNode();
                if (sceneNode != null)
                {
                    scale = _projectionManager
                        .ProjectFrom(sceneNode)
                        .To(ScaleAnchorNode)
                        .InDirections(GraphDirections.Predecessors)
                        .FirstOrDefault()?.Value.Scale;
                }
            }

            float u = Unit / (scale ?? 1);
            var v = new Vector2(u, u);
            
            _positionHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, 0), liveRectangle.Size);

            _leftHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(-u, 0), new Vector2(u, liveRectangle.Size.Y));
            _topHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, -u), new Vector2(liveRectangle.Size.X, u));
            _rightHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, 0), new Vector2(u, liveRectangle.Size.Y));
            _bottomHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, 0), new Vector2(liveRectangle.Size.X, u));
            
            _topLeftHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(-u, -u), v);
            _topRightHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(0, -u), v);
            _bottomLeftHandleSetter.Rectangle = new TopLeftRectangle(new Vector2(-u, 0), v);
            _bottomRightHandleSetter.Rectangle = new TopLeftRectangle(Vector2.Zero, v);
        }

        public class Anchor : AnchoredEditorBase<IAnchoredRectangleController>
        {
            private readonly List<AdvancedRectanglePositionHandle> _positionHandles;
            private readonly List<AdvancedRectangleBorderPositionHandle> _borderHandles;

            private IEnumerable<IHandle<IAnchoredRectangleController>> AllHandles
                => _positionHandles.Concat<IHandle<IAnchoredRectangleController>>(_borderHandles);

            public override IEnumerable<IHandle> Handles => AllHandles;
            
            public ITransformation AnchorTransformation
            {
                get => AnchoredSceneNode.AnchorTransformation;
                set => AnchoredSceneNode.AnchorTransformation = value;
            }

            public Func<Vector2, Vector2> Revaluation
            {
                get => _borderHandles[0].Revaluation;
                set
                {
                    foreach (AdvancedRectanglePositionHandle positionHandle in _positionHandles)
                        positionHandle.Revaluation = value;
                    foreach (AdvancedRectangleBorderPositionHandle borderHandle in _borderHandles)
                        borderHandle.Revaluation = value;
                }
            }

            public Anchor(GlyphResolveContext context)
                : base(context)
            {
                _positionHandles = new List<AdvancedRectanglePositionHandle>();
                _borderHandles = new List<AdvancedRectangleBorderPositionHandle>();
            }

            public AdvancedRectanglePositionHandle CreatePositionHandle()
            {
                var handle = Add<AdvancedRectanglePositionHandle>();

                _positionHandles.Add(handle);
                return handle;
            }

            public AdvancedRectangleBorderPositionHandle CreateBorderHandle()
            {
                var handle = Add<AdvancedRectangleBorderPositionHandle>();

                _borderHandles.Add(handle);
                return handle;
            }

            protected override void AssignEditedObjectToHandles(IAnchoredRectangleController editedObject)
            {
                foreach (IHandle<IAnchoredRectangleController> handle in AllHandles)
                    handle.EditedObject = editedObject;
            }
        }

        public class HandleSetter
        {
            private readonly AdvancedHandleBase<IAnchoredRectangleController> _handle;
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

            public HandleSetter(AdvancedHandleBase<IAnchoredRectangleController> handle)
            {
                _handle = handle;
                _backgroundMesh = new TriangulableShapeMesh<TopLeftRectangle>(Color.White * 0.15f, TopLeftRectangle.Void);
                _borderMesh = new EdgedShapeOutlineMesh<TopLeftRectangle>(Color.White, TopLeftRectangle.Void);

                _handle.DefaultMeshes.Add(_backgroundMesh);
                _handle.DefaultMeshes.Add(_borderMesh);
            }
        }
    }
}