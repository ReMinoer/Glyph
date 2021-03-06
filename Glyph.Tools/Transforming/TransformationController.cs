﻿using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class TransformationController : IAnchoredTransformationController, IAnchoredPositionController, IAnchoredRotationController, IAnchoredScaleController
    {
        public IWritableSceneNode SceneNode { get; }
        public bool OrientedReferential { get; }

        ISceneNode IAnchoredController.Anchor => SceneNode;

        public TransformationController(IWritableSceneNode sceneNode, bool orientedReferential)
        {
            SceneNode = sceneNode;
            OrientedReferential = orientedReferential;
        }

        Vector2 IPositionController.Position
        {
            get => SceneNode.Position;
            set => SceneNode.Position = value;
        }

        float IRotationController.Rotation
        {
            get => SceneNode.Rotation;
            set => SceneNode.Rotation = value;
        }

        float IScaleController.Scale
        {
            get => SceneNode.Scale;
            set => SceneNode.Scale = value;
        }

        IPositionController ITransformationController.PositionController => this;
        IRotationController ITransformationController.RotationController => this;
        IScaleController ITransformationController.ScaleController => this;

        IAnchoredPositionController IAnchoredTransformationController.PositionController => this;
        IAnchoredRotationController IAnchoredTransformationController.RotationController => this;
        IAnchoredScaleController IAnchoredTransformationController.ScaleController => this;
    }
}