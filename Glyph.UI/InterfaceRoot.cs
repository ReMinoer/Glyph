using Fingear.Interactives;
using Fingear.Interactives.Interfaces;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.UI
{
    public class InterfaceRoot : InteractiveChildComponentBase<InteractiveInterfaceRoot, IInteractive>
    {
        private readonly ProjectionCursorControl _cursorProjection;
        public override sealed InteractiveInterfaceRoot Interactive { get; } = new InteractiveInterfaceRoot();

        public IDrawClient RaycastClient
        {
            get => _cursorProjection.RaycastClient;
            set => _cursorProjection.RaycastClient = value;
        }

        public InterfaceRoot(RootView rootView, ProjectionManager projectionManager, [Resolvable, ResolveTargets(ResolveTargets.Parent)] IGlyphComponent parent = null)
        {
            if (parent?.Name != null)
                Interactive.Name = parent.Name + " interface root";

            Interactive.Cursor = _cursorProjection = new ProjectionCursorControl("Scene cursor", InputSystem.Instance.Mouse.Cursor, rootView, new ReadOnlySceneNodeDelegate(() => this.GetSceneNode().RootNode()), projectionManager);
            Interactive.Touch = UserInterfaceControls.Instance.Touch;
            Interactive.Direction = UserInterfaceControls.Instance.Direction;
            Interactive.Confirm = UserInterfaceControls.Instance.Confirm;
            Interactive.Cancel = UserInterfaceControls.Instance.Cancel;
            Interactive.Exit = UserInterfaceControls.Instance.Exit;
        }
    }
}