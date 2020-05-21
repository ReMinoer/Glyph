using Glyph.Core;
using Glyph.Graphics.Primitives;
using Glyph.Graphics.Renderer;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.UI;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class MultiModeTransformationEditor : GlyphObject, IIntegratedEditor<IMultiModeAnchoredTransformationController>
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

        public MultiModeTransformationEditor(GlyphResolveContext context)
            : base(context)
        {
            _transformationEditor = Add<TransformationEditor>();

            var modeToggle = Add<ModeToggle>();
            modeToggle.Parent = _transformationEditor;
        }

        public class ModeToggle : GlyphObject
        {
            private readonly MultiModeTransformationEditor _editor;

            private readonly SceneNode _sceneNode;
            private readonly CenteredRectangle _primitive = new CenteredRectangle(Vector2.Zero, new Vector2(TransformationEditor.Unit / 2, TransformationEditor.Unit / 2));

            private Quad InteractiveArea => _sceneNode.Transform(_primitive);

            public ModeToggle(GlyphResolveContext context, MultiModeTransformationEditor editor)
                : base(context)
            {
                _editor = editor;

                _sceneNode = Add<SceneNode>();
                _sceneNode.LocalPosition = new Vector2(-TransformationEditor.Unit / 4, -TransformationEditor.Unit / 4);

                var userInterface = Add<UserInterface>();
                userInterface.TouchStarted += OnTouchStarted;
                userInterface.TouchEnded += OnTouchEnded;

                Add<PrimitiveRenderer>().PrimitiveProviders.Add(_primitive.ToPrimitive(Color.Pink));
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