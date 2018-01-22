using Fingear;
using Fingear.MonoGame;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Core.Inputs
{
    public class SceneCursorControl : CursorControlBase
    {
        private readonly InputClientManager _inputClientManager;
        private readonly ViewManager _viewManager;

        public SceneCursorControl(InputClientManager inputClientManager, ViewManager viewManager)
            : this(null, null, inputClientManager, viewManager)
        {
        }

        public SceneCursorControl(ICursorInput input, InputClientManager inputClientManager, ViewManager viewManager)
            : this(null, input, inputClientManager, viewManager)
        {
        }

        public SceneCursorControl(string name, ICursorInput input, InputClientManager inputClientManager, ViewManager viewManager)
            : base(name, input)
        {
            _inputClientManager = inputClientManager;
            _viewManager = viewManager;
        }

        protected override bool ComputeCursor(out System.Numerics.Vector2 value)
        {
            Vector2 virtualPosition = _inputClientManager.Current.Resolution.WindowToVirtualScreen(Input.Value.AsMonoGamePoint());
            IView view = _viewManager.GetViewAtPoint(virtualPosition, _inputClientManager.Current as IDrawClient, out Vector2 viewPosition);
            if (view == null)
            {
                value = default;
                return false;
            }

            value = view.ViewToScene(viewPosition).AsSystemVector();
            return true;
        }
    }
}