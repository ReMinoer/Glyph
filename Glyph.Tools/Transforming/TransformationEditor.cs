﻿using System.Collections.Generic;
using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class TransformationEditor : GlyphObject, IIntegratedEditor<IAnchoredTransformationController>
    {
        public const float Unit = 100;

        private readonly AnchoredSceneNode _anchoredSceneNode;
        private readonly List<AdvancedPositionHandle> _positionHandles;
        private readonly List<AdvancedRotationHandle> _rotationHandles;
        private readonly List<AdvancedScaleHandle> _scaleHandles;

        private IEnumerable<IHandle> Handles
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

        private IAnchoredTransformationController _editedObject;
        public IAnchoredTransformationController EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                _anchoredSceneNode.AnchorNode = _editedObject.Anchor;
                _anchoredSceneNode.IgnoreRotation = !_editedObject.OrientedReferential;

                ConfigureHandles(_positionHandles, _editedObject.PositionController);
                ConfigureHandles(_rotationHandles, _editedObject.RotationController);
                ConfigureHandles(_scaleHandles, _editedObject.ScaleController);

                void ConfigureHandles<THandle, TController>(IEnumerable<THandle> handles, TController controller)
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
        
        object IIntegratedEditor.EditedObject => EditedObject;

        private IDrawClient _raycastClient;
        public IDrawClient RaycastClient
        {
            get => _raycastClient;
            set
            {
                _raycastClient = value;

                foreach (IHandle handle in Handles)
                    handle.RaycastClient = _raycastClient;
            }
        }

        public TransformationEditor(GlyphResolveContext context)
            : base(context)
        {
            _anchoredSceneNode = Add<AnchoredSceneNode>();
            _anchoredSceneNode.IgnoreRotation = true;
            _anchoredSceneNode.IgnoreScale = true;
            _anchoredSceneNode.ProjectionConfiguration = x => x.WithViewDepthMax(1);

            Add<UserInterface>();

            const float u = Unit;
            const float cursorSize = u / 16;
            float radius = (new Vector2(2.5f, 2.5f) * u).Length();

            var positionHandle = Add<AdvancedPositionHandle>();
            positionHandle.Rectangle = new TopLeftRectangle(Vector2.Zero, new Vector2(1.5f, 1.5f) * u);
            positionHandle.DefaultPrimitives.Add(new TopLeftRectangle(Vector2.Zero, new Vector2(1.5f, 1.5f) * u).ToPrimitive(Color.White * 0.25f));
            positionHandle.DefaultPrimitives.Add(new LinePrimitive(Color.White * 0.5f, new Vector2(1.5f, 0) * u, new Vector2(1.5f, 1.5f) * u, new Vector2(0, 1.5f) * u));

            var scaleHandle = Add<AdvancedScaleHandle>();
            scaleHandle.LocalPosition = new Vector2(1.5f, 1.5f) * u;
            scaleHandle.Rectangle = new CenteredRectangle(Vector2.Zero, new Vector2(1f, 1f) * u);
            scaleHandle.DefaultPrimitives.Add(new LinePrimitive(Color.Black, new Vector2(0.5f, 0) * u, new Vector2(0.5f, 0.5f) * u, new Vector2(0, 0.5f) * u));
            scaleHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Black, new Vector2(0.5f, 0.5f) * u, radius: cursorSize));

            var horizontalHandle = Add<AdvancedPositionHandle>();
            horizontalHandle.Rectangle = new CenteredRectangle(new Vector2(1.75f, 0) * u, new Vector2(3.5f, 2) * u);
            horizontalHandle.Axes = Axes.Horizontal;
            horizontalHandle.DefaultPrimitives.Add(new LinePrimitive(Color.Red, Vector2.Zero, new Vector2(3, 0) * u));
            horizontalHandle.DefaultPrimitives.Add(new EllipsePrimitive(Color.Red, new Vector2(3, 0) * u, u / 4, sampling: 3));
            horizontalHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Red, Vector2.Zero, cursorSize));
            horizontalHandle.GrabbedPrimitives.Add(new LinePrimitive(Color.Red, -Vector2.UnitX * float.MaxValue, Vector2.Zero, Vector2.UnitX * float.MaxValue));

            var verticalHandle = Add<AdvancedPositionHandle>();
            verticalHandle.Rectangle = new CenteredRectangle(new Vector2(0, 1.75f) * u, new Vector2(2, 3.5f) * u);
            verticalHandle.Axes = Axes.Vertical;
            verticalHandle.DefaultPrimitives.Add(new LinePrimitive(Color.Blue, Vector2.Zero, new Vector2(0, 3) * u));
            verticalHandle.DefaultPrimitives.Add(new EllipsePrimitive(Color.Blue, new Vector2(0, 3) * u, u / 4, rotation: MathHelper.PiOver2, sampling: 3));
            verticalHandle.HoverPrimitives.Add(new EllipsePrimitive(Color.Blue, Vector2.Zero, cursorSize));
            verticalHandle.GrabbedPrimitives.Add(new LinePrimitive(Color.Blue, -Vector2.UnitY * float.MaxValue, Vector2.Zero, Vector2.UnitY * float.MaxValue));

            var rotationHandle = Add<AdvancedRotationHandle>();
            rotationHandle.Rectangle = new TopLeftRectangle(new Vector2(1, 1) * u, new Vector2(3, 3) * u);
            rotationHandle.DefaultPrimitives.Add(new EllipseOutlinePrimitive(Color.Green, Vector2.Zero, radius, angleStart: MathHelper.ToRadians(15), angleSize: MathHelper.ToRadians(60)));
            rotationHandle.GrabbedPrimitives.Add(new EllipseOutlinePrimitive(Color.Green, Vector2.Zero, radius, sampling: EllipsePrimitive.DefaultSampling * 2));

            EllipsePrimitive rotationCursor;
            rotationHandle.HoverPrimitives.Add(rotationCursor = new EllipsePrimitive(Color.Green, Vector2.UnitY * radius, cursorSize));
            rotationHandle.Schedulers.Update.Plan(_ =>
            {
                if (_editedObject.OrientedReferential)
                    rotationCursor.Center = _anchoredSceneNode.AnchorNode.Rotation.ToRotatedVector() * radius;
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

            Schedulers.Draw.Plan(horizontalHandle).After(positionHandle);
            Schedulers.Draw.Plan(verticalHandle).After(positionHandle);
            Schedulers.Draw.Plan(scaleHandle).After(positionHandle);
            Schedulers.Draw.Plan(positionHandle).After(rotationHandle);
        }
    }
}