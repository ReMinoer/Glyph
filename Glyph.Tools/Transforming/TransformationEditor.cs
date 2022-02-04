using System.Collections.Generic;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class TransformationEditor : AnchoredEditorBase<IAnchoredTransformationController>
    {
        public const float Unit = 100;
        
        private readonly List<AdvancedPositionHandle> _positionHandles;
        private readonly List<AdvancedRotationHandle> _rotationHandles;
        private readonly List<AdvancedScaleHandle> _scaleHandles;

        public override IEnumerable<IHandle> Handles
        {
            get
            {
                foreach (AdvancedPositionHandle handle in _positionHandles)
                    yield return handle;
                foreach (AdvancedRotationHandle handle in _rotationHandles)
                    yield return handle;
                foreach (AdvancedScaleHandle handle in _scaleHandles)
                    yield return handle;
            }
        }
        
        public override IAnchoredTransformationController EditedObject
        {
            get => base.EditedObject;
            set
            {
                base.EditedObject = value;
                AnchoredSceneNode.IgnoreRotation = !EditedObject.OrientedReferential;
            }
        }

        public TransformationEditor(GlyphResolveContext context)
            : base(context)
        {
            const float u = Unit;
            const float cursorSize = u / 16;
            float radius = (new Vector2(2.5f, 2.5f) * u).Length();

            var positionHandle = Add<AdvancedPositionHandle>();
            positionHandle.Rectangle = new TopLeftRectangle(Vector2.Zero, new Vector2(1.5f, 1.5f) * u);
            positionHandle.DefaultMeshes.Add(new TopLeftRectangle(Vector2.Zero, new Vector2(1.5f, 1.5f) * u).ToMesh(Color.White * 0.25f));
            positionHandle.DefaultMeshes.Add(new LineMesh(Color.White * 0.5f, new Vector2(1.5f, 0) * u, new Vector2(1.5f, 1.5f) * u, new Vector2(0, 1.5f) * u));
            positionHandle.KeyboardEnabled = true;

            var scaleHandle = Add<AdvancedScaleHandle>();
            scaleHandle.LocalPosition = new Vector2(1.5f, 1.5f) * u;
            scaleHandle.Rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(1f, 1f) * u);
            scaleHandle.DefaultMeshes.Add(new LineMesh(Color.Black, new Vector2(0.5f, 0) * u, new Vector2(0.5f, 0.5f) * u, new Vector2(0, 0.5f) * u));
            scaleHandle.HoverMeshes.Add(new EllipseMesh(Color.Black, new Vector2(0.5f, 0.5f) * u, radius: cursorSize));

            var horizontalHandle = Add<AdvancedPositionHandle>();
            horizontalHandle.Rectangle = new CenteredRectangle(new Vector2(1.75f, 0) * u, new Vector2(3.5f, 2) * u);
            horizontalHandle.Axes = Axes.Horizontal;
            horizontalHandle.DefaultMeshes.Add(new LineMesh(Color.Red, Vector2.Zero, new Vector2(3, 0) * u));
            horizontalHandle.DefaultMeshes.Add(new EllipseMesh(Color.Red, new Vector2(3, 0) * u, u / 4, sampling: 3));
            horizontalHandle.HoverMeshes.Add(new EllipseMesh(Color.Red, Vector2.Zero, cursorSize));
            horizontalHandle.GrabbedMeshes.Add(new LineMesh(Color.Red, -Vector2.UnitX * float.MaxValue, Vector2.Zero, Vector2.UnitX * float.MaxValue));

            var verticalHandle = Add<AdvancedPositionHandle>();
            verticalHandle.Rectangle = new CenteredRectangle(new Vector2(0, 1.75f) * u, new Vector2(2, 3.5f) * u);
            verticalHandle.Axes = Axes.Vertical;
            verticalHandle.DefaultMeshes.Add(new LineMesh(Color.Blue, Vector2.Zero, new Vector2(0, 3) * u));
            verticalHandle.DefaultMeshes.Add(new EllipseMesh(Color.Blue, new Vector2(0, 3) * u, u / 4, rotation: MathHelper.PiOver2, sampling: 3));
            verticalHandle.HoverMeshes.Add(new EllipseMesh(Color.Blue, Vector2.Zero, cursorSize));
            verticalHandle.GrabbedMeshes.Add(new LineMesh(Color.Blue, -Vector2.UnitY * float.MaxValue, Vector2.Zero, Vector2.UnitY * float.MaxValue));

            var rotationHandle = Add<AdvancedRotationHandle>();
            rotationHandle.Rectangle = new TopLeftRectangle(new Vector2(1, 1) * u, new Vector2(3, 3) * u);
            rotationHandle.DefaultMeshes.Add(new EllipseOutlineMesh(Color.Green, Vector2.Zero, radius, angleStart: MathHelper.ToRadians(15), angleSize: MathHelper.ToRadians(60)));
            rotationHandle.GrabbedMeshes.Add(new EllipseOutlineMesh(Color.Green, Vector2.Zero, radius, sampling: EllipseMesh.DefaultSampling * 2));

            EllipseMesh rotationCursor;
            rotationHandle.HoverMeshes.Add(rotationCursor = new EllipseMesh(Color.Green, Vector2.UnitY * radius, cursorSize));
            rotationHandle.Schedulers.Update.Plan(_ =>
            {
                if (EditedObject.OrientedReferential)
                    rotationCursor.Center = AnchoredSceneNode.AnchorNode.Rotation.ToRotatedVector() * radius;
                else
                    rotationCursor.Center = Vector2.UnitX * radius;
            });

            _positionHandles = new List<AdvancedPositionHandle>
            {
                positionHandle,
                horizontalHandle,
                verticalHandle
            };

            _rotationHandles = new List<AdvancedRotationHandle>
            {
                rotationHandle
            };

            _scaleHandles = new List<AdvancedScaleHandle>
            {
                scaleHandle
            };
        }

        protected override void AssignEditedObjectToHandles(IAnchoredTransformationController editedObject)
        {
            ConfigureHandles(_positionHandles, EditedObject.PositionController);
            ConfigureHandles(_rotationHandles, EditedObject.RotationController);
            ConfigureHandles(_scaleHandles, EditedObject.ScaleController);
        }

        static private void ConfigureHandles<THandle, TController>(IEnumerable<THandle> handles, TController controller)
            where THandle : class, IHandle<TController>
        {
            bool hasController = controller != null;
            foreach (THandle handle in handles)
            {
                handle.EditedObject = controller;
                handle.Enabled = hasController;
                handle.Visible = hasController;
            }
        }
    }
}