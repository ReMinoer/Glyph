using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input
{
    public class MouseManager
    {
        private Vector2 _windowSize;
        static private MouseManager _instance;
        public Point DefaultPosition { get; set; }

        static public MouseManager Instance
        {
            get { return _instance ?? (_instance = new MouseManager()); }
        }

        protected MouseManager()
        {
            DefaultPosition = (Resolution.Instance.Size / 2).ToPoint();
            Resolution.Instance.SizeChanged += OnResolutionChanged;
        }

        public void ResetPosition()
        {
            Mouse.SetPosition(DefaultPosition.X, DefaultPosition.Y);
        }

        public void SetPosition(Point position)
        {
            Mouse.SetPosition(position.X, position.Y);
        }

        private void OnResolutionChanged(object sender, Resolution.SizeChangedEventArgs e)
        {
            Vector2 scale = e.NewSize / _windowSize;
            DefaultPosition = DefaultPosition.ToVector2().Multiply(scale.X, scale.Y).ToPoint();
            _windowSize = e.NewSize;
        }
    }
}