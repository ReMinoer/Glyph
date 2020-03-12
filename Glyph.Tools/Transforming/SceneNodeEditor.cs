using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class SceneNodeEditor : GlyphObject, IIntegratedEditor<IWritableSceneNodeComponent>
    {
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

            const float u = 100;
            const float handleSize = u / 16;
            float radius = (new Vector2(2.5f, 2.5f) * u).Length();

            _scaleHandle = Add<AdvancedScaleHandle>();
            _scaleHandle.LocalPosition = new Vector2(2, 2) * u;
            _scaleHandle.Rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(1, 1) * u);
            _scaleHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Black, Vector2.Zero, radius: handleSize));

            _positionHandle = Add<AdvancedPositionHandle>();
            _positionHandle.Rectangle = new CenteredRectangle(new Vector2(0.75f, 0.75f) * u, new Vector2(1.5f, 1.5f) * u);

            _horizontalHandle = Add<AdvancedPositionHandle>();
            _horizontalHandle.Rectangle = new CenteredRectangle(new Vector2(1.75f, 0) * u, new Vector2(3.5f, 2) * u);
            _horizontalHandle.Axes = Axes.Horizontal;
            _horizontalHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Red, Vector2.Zero, handleSize));
            _horizontalHandle.GrabbedPrimitives.Add(new LinePrimitive(Color.Red, -Vector2.UnitX * float.MaxValue, Vector2.UnitX * float.MaxValue));

            _verticalHandle = Add<AdvancedPositionHandle>();
            _verticalHandle.Rectangle = new CenteredRectangle(new Vector2(0, 1.75f) * u, new Vector2(2, 3.5f) * u);
            _verticalHandle.Axes = Axes.Vertical;
            _verticalHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Blue, Vector2.Zero, handleSize));
            _verticalHandle.GrabbedPrimitives.Add(new LinePrimitive(Color.Blue, -Vector2.UnitY * float.MaxValue, Vector2.UnitY * float.MaxValue));

            _rotationHandle = Add<AdvancedRotationHandle>();
            _rotationHandle.Rectangle = new CenteredRectangle(new Vector2(2, 2) * u, new Vector2(3, 3) * u);
            _rotationHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Green, Vector2.UnitY * radius, handleSize));
            _rotationHandle.GrabbedPrimitives.Add(new EllipseOutlinePrimitive(Color.Green, Vector2.Zero, radius, sampling: EllipsePrimitive.DefaultSampling * 2));

            Schedulers.Draw.Plan(_horizontalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_verticalHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_scaleHandle).After(_positionHandle);
            Schedulers.Draw.Plan(_positionHandle).After(_rotationHandle);

            var lineObject = Add<GlyphObject>();
            lineObject.Add<SceneNode>();
            lineObject.Add<PrimitiveRenderer>().Primitives.AddRange(new IPrimitive[]
            {
                new TopLeftRectangle(Vector2.Zero, new Vector2(1.5f, 1.5f) * u).ToPrimitive(Color.White * 0.25f),
                new LinePrimitive(Color.White * 0.5f, new Vector2(1.5f, 0) * u, new Vector2(1.5f, 1.5f) * u),
                new LinePrimitive(Color.White * 0.5f, new Vector2(0, 1.5f) * u, new Vector2(1.5f, 1.5f) * u),
                new LinePrimitive(Color.Red, Vector2.Zero, new Vector2(3, 0) * u),
                new EllipsePrimitive(Color.Red, new Vector2(3, 0) * u, u / 4, sampling: 3),
                new LinePrimitive(Color.Blue, Vector2.Zero, new Vector2(0, 3) * u),
                new EllipsePrimitive(Color.Blue, new Vector2(0, 3) * u, u / 4, rotation: MathHelper.PiOver2, sampling: 3),
                new EllipseOutlinePrimitive(Color.Green, Vector2.Zero, radius, angleStart: MathHelper.ToRadians(15), angleSize: MathHelper.ToRadians(60)),
                new LinePrimitive(Color.Black, new Vector2(2, 1.5f) * u, new Vector2(2, 2) * u, new Vector2(1.5f, 2) * u)
            });
        }
    }
}