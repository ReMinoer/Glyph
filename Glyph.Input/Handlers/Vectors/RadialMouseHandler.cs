using System.Linq;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public class RadialMouseHandler : VectorHandler
    {
        public float VirtualRadius { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.Mouse; }
        }

        public RadialMouseHandler()
            : this("", 0)
        {
        }

        public RadialMouseHandler(string name, float virtualRadius, float deadZone = 0, bool inverseX = false,
            bool inverseY = false, InputActivity inputActivity = InputActivity.Pressed)
            : base(name, deadZone, inverseX, inverseY, inputActivity)
        {
            VirtualRadius = virtualRadius;
        }

        protected override Vector2 GetState(InputStates inputStates)
        {
            Point windowPoint = inputStates.MouseState.Position;
            IView view = ViewManager.Main.GetViewAtWindowPoint(windowPoint) ?? ViewManager.Main.Views.First();

            return (windowPoint.ToVector2() - MouseManager.Instance.DefaultPosition.ToVector2())
                   / (VirtualRadius * (Resolution.Instance.ScaleRatio * view.Camera.Zoom));
        }
    }
}