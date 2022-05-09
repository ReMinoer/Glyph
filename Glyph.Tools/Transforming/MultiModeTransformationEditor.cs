using System;
using Glyph.Core;
using Glyph.Graphics.Meshes;
using Glyph.Graphics.Renderer;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class MultiModeTransformationEditor : GlyphObject, IHandle<IMultiModeAnchoredTransformationController>
    {
        private readonly TransformationEditor _transformationEditor;

        private IMultiModeAnchoredTransformationController _editedObject;
        public IMultiModeAnchoredTransformationController EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                _transformationEditor.EditedObject = _editedObject.Modes[0];
            }
        }

        object IIntegratedEditor.EditedObject => EditedObject;

        public IDrawClient RaycastClient
        {
            get => _transformationEditor.RaycastClient;
            set => _transformationEditor.RaycastClient = value;
        }

        public ISceneNode ParentNode
        {
            get => _transformationEditor.ParentNode;
            set => _transformationEditor.ParentNode = value;
        }

        public Func<Vector2, Vector2> Revaluation
        {
            get => _transformationEditor.Revaluation;
            set => _transformationEditor.Revaluation = value;
        }

        private int _selectedModeIndex;
        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set
            {
                _selectedModeIndex = value;
                _transformationEditor.EditedObject = EditedObject.Modes[_selectedModeIndex];
            }
        }

        public event EventHandler Grabbed;
        public event EventHandler Dragging;
        public event EventHandler Released;
        public event EventHandler Cancelled;

        public MultiModeTransformationEditor(GlyphResolveContext context)
            : base(context)
        {
            _transformationEditor = Add<TransformationEditor>();
            _transformationEditor.Grabbed += OnGrabbed;
            _transformationEditor.Dragging += OnDragging;
            _transformationEditor.Released += OnReleased;
            _transformationEditor.Cancelled += OnCancelled;

            var modeToggle = Add<ModeToggle>();
            modeToggle.Parent = _transformationEditor;
        }

        public override void Dispose()
        {
            _transformationEditor.Cancelled -= OnCancelled;
            _transformationEditor.Released -= OnReleased;
            _transformationEditor.Dragging -= OnDragging;
            _transformationEditor.Grabbed -= OnGrabbed;

            base.Dispose();
        }

        private void OnGrabbed(object sender, EventArgs e) => Grabbed?.Invoke(this, EventArgs.Empty);
        private void OnDragging(object sender, EventArgs e) => Dragging?.Invoke(this, EventArgs.Empty);
        private void OnReleased(object sender, EventArgs e) => Released?.Invoke(this, EventArgs.Empty);
        private void OnCancelled(object sender, EventArgs e) => Cancelled?.Invoke(this, EventArgs.Empty);

        public class ModeToggle : GlyphObject
        {
            private readonly MultiModeTransformationEditor _editor;

            private readonly SceneNode _sceneNode;
            private readonly CenteredRectangle _buttonShape = new CenteredRectangle(Vector2.Zero, new Vector2(TransformationEditor.Unit / 2, TransformationEditor.Unit / 2));

            private Quad InteractiveArea => _sceneNode.Transform(_buttonShape);

            public ModeToggle(GlyphResolveContext context, MultiModeTransformationEditor editor)
                : base(context)
            {
                _editor = editor;

                _sceneNode = Add<SceneNode>();
                _sceneNode.LocalPosition = new Vector2(-TransformationEditor.Unit / 4, -TransformationEditor.Unit / 4);

                var userInterface = Add<UserInterface>();
                userInterface.TouchStarted += OnTouchStarted;
                userInterface.TouchEnded += OnTouchEnded;

                Add<MeshRenderer>().MeshProviders.Add(_buttonShape.ToMesh(Color.Pink));
            }

            private void OnTouchStarted(object sender, HandlableTouchEventArgs e)
            {
                if (InteractiveArea.ContainsPoint(e.CursorPosition))
                    e.Handle();
            }

            private void OnTouchEnded(object sender, CursorEventArgs e)
            {
                if (!InteractiveArea.ContainsPoint(e.CursorPosition))
                    return;
                
                _editor.SelectedModeIndex = (_editor.SelectedModeIndex + 1) % _editor.EditedObject.Modes.Count;
            }
        }
    }
}