using System.Collections.Generic;
using System.Linq;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Meshes;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.Tools.Transforming;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Parallax
{
    public class ParallaxManipulator : GlyphObject
    {
        static public readonly Color AreaColor = Color.Cyan;

        private readonly EdgedShapeOutlineMesh<TopLeftRectangle> _parallaxAreaOutlineMesh;
        private readonly PointOfView _pointOfView;
        private readonly PointOfViewEditor _pointOfViewEditor;

        private IParallaxManipulatorSettings _settings;
        public IParallaxManipulatorSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings == value)
                    return;

                if (_settings != null)
                    _settings.ParallaxRoot.PointOfView = null;

                _settings = value;
                _pointOfViewEditor.Settings = value;

                if (_settings != null)
                    _settings.ParallaxRoot.PointOfView = _pointOfView.SceneNode;
            }
        }

        public ParallaxManipulator(GlyphResolveContext context)
            : base(context)
        {
            Add<SceneNode>();

            _parallaxAreaOutlineMesh = new EdgedShapeOutlineMesh<TopLeftRectangle>(AreaColor);
            Add<MeshesComponent>().Meshes.Add(_parallaxAreaOutlineMesh);
            Add<MeshRenderer>();

            _pointOfView = Add<PointOfView>();
            _pointOfViewEditor = Add<PointOfViewEditor>();
            _pointOfViewEditor.EditedObject = new TransformationController(_pointOfView.SceneNode, false);

            Schedulers.Initialize.Plan(UpdateShape);
            Schedulers.Update.Plan(UpdateShape);
        }

        private void UpdateShape(ElapsedTime elapsedTime) => UpdateShape();
        private void UpdateShape()
        {
            if (Settings == null)
                return;

            _parallaxAreaOutlineMesh.Shape = Settings.MainLayerRectangle;
        }

        public class PointOfView : GlyphObject
        {
            public SceneNode SceneNode { get; }

            public PointOfView(GlyphResolveContext context)
                : base(context)
            {
                SceneNode = Add<SceneNode>();
            }
        }

        public class PointOfViewEditor : AnchoredEditorBase<IAnchoredPositionController>
        {
            private const float Size = 100;
            static private readonly Color Color = Color.Cyan;
            static private readonly float ArcRadius = Size / 2 * (float)System.Math.Sqrt(2);
            static private readonly float IrisRadius = Size / 2 - ArcRadius;

            private readonly ProjectionManager _projectionManager;

            public IParallaxManipulatorSettings Settings { get; set; }

            private readonly AdvancedPositionHandle _positionHandle;
            private readonly EdgedShapeOutlineMesh<CenteredRectangle> _displayedRectangleOutline;

            public override IEnumerable<IHandle> Handles
            {
                get
                {
                    yield return _positionHandle;
                }
            }

            protected override void AssignEditedObjectToHandles(IAnchoredPositionController editedObject)
            {
                _positionHandle.EditedObject = editedObject;
            }

            public PointOfViewEditor(GlyphResolveContext context, ProjectionManager projectionManager)
                : base(context)
            {
                _projectionManager = projectionManager;

                _positionHandle = Add<AdvancedPositionHandle>();
                _positionHandle.Revaluation = ClampToCameraArea;
                _positionHandle.Rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(Size, Size / 2));

                // Eye
                _positionHandle.DefaultMeshes.Add(new EllipseOutlineMesh(Color,  Size / 2 * Vector2.UnitY, ArcRadius,
                    angleStart: MathHelper.Pi + MathHelper.PiOver4, angleSize: MathHelper.PiOver2));
                _positionHandle.DefaultMeshes.Add(new EllipseOutlineMesh(Color, -Size / 2 * Vector2.UnitY, ArcRadius,
                    angleStart: MathHelper.PiOver4, angleSize: MathHelper.PiOver2));

                // Iris
                _positionHandle.DefaultMeshes.Add(new EllipseOutlineMesh(Color, Vector2.Zero, IrisRadius));

                // Cross
                _positionHandle.HoverMeshes.Add(new LineMesh(Color, Size / 8 * Vector2.UnitX, -Size / 8 * Vector2.UnitX));
                _positionHandle.HoverMeshes.Add(new LineMesh(Color, Size / 8 * Vector2.UnitY, -Size / 8 * Vector2.UnitY));

                // Displayed rectangle
                _displayedRectangleOutline = new EdgedShapeOutlineMesh<CenteredRectangle>(Color);
                _positionHandle.HoverMeshes.Add(_displayedRectangleOutline);

                Schedulers.Initialize.Plan(UpdateClamping);
                Schedulers.Initialize.Plan(UpdateDisplayedRectangleOutline);
                Schedulers.Update.Plan(UpdateClamping);
                Schedulers.Update.Plan(UpdateDisplayedRectangleOutline);
            }

            private void UpdateClamping(ElapsedTime elapsedTime) => UpdateClamping();
            private void UpdateClamping()
            {
                EditedObject.Position = ClampToCameraArea(EditedObject.Position);
            }

            private Vector2 ClampToCameraArea(Vector2 handlePosition)
            {
                ISceneNode handleReferential = AnchoredSceneNode;
                ISceneNode parallaxReferential = Settings.ParallaxRoot.Referential;

                Vector2? parallaxPosition = _projectionManager.ProjectFromPosition(handleReferential, handlePosition).To(parallaxReferential).FirstOrDefault()?.Value;
                if (parallaxPosition is null)
                    return handlePosition;

                Vector2 clampedParallaxPosition = parallaxPosition.Value.ClampToRectangle(Settings.MainLayerRectangle.Inflate(-Settings.DisplayedSize));

                Vector2? clampedHandlePosition = _projectionManager.ProjectFromPosition(parallaxReferential, clampedParallaxPosition).To(handleReferential).FirstOrDefault()?.Value;
                if (clampedHandlePosition is null)
                    return handlePosition;

                return clampedHandlePosition.Value;
            }

            private void UpdateDisplayedRectangleOutline(ElapsedTime elapsedTime) => UpdateClamping();
            private void UpdateDisplayedRectangleOutline()
            {
                if (Settings != null)
                    _displayedRectangleOutline.Shape = new CenteredRectangle(Vector2.Zero, Settings.DisplayedSize);
                else
                    _displayedRectangleOutline.Shape = CenteredRectangle.Void;
            }
        }
    }
}