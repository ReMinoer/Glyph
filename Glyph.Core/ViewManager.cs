using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class ViewManager : GlyphContainer
    {
        static private ViewManager _main;
        static public ViewManager Main
        {
            get { return _main ?? (_main = new ViewManager()); }
        }

        private SceneNode _sceneNode;
        private List<IView> _views;
        private IReadOnlyCollection<IView> _readOnlyScreens;

        public IEnumerable<IView> Views
        {
            get { return _views; }
        }

        public ViewManager()
        {
            _sceneNode = new SceneNode();
            _views = new List<IView>();
            _readOnlyScreens = _views.AsReadOnly();
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (IView screen in Views)
                screen.Update(elapsedTime);
        }

        public void RegisterScreen(IView view)
        {
            Components.Add(view);
            _views.Add(view);

            view.Initialize();
        }

        public void UnregisterScreen(IView view)
        {
            _views.Remove(view);
        }

        public IView GetViewAtWindowPoint(Point windowPoint)
        {
            return Views.Reverse().FirstOrDefault(view => view.BoundingBox.ContainsPoint(windowPoint.ToVector2()));
        }

        static public bool IsVisibleOnWindow(Vector2 position)
        {
            return Main.Views.Any(view => view.IsVisibleOnView(position));
        }

        static public bool IsVisibleOnWindow(Vector2 position, out IView view)
        {
            foreach (IView v in Main.Views)
            {
                if (!v.IsVisibleOnView(position))
                    continue;

                view = v;
                return true;
            }

            view = null;
            return false;
        }

        static public Vector2 GetPositionOnWindow(Vector2 position, IView view)
        {
            return view.GetPositionOnView(position) + view.BoundingBox.Origin + Resolution.Instance.WindowMargin;
        }

        static public IEnumerable<KeyValuePair<IView, Vector2>> GetAllPositionsOnWindow(Vector2 position)
        {
            return Main.Views.Select(view =>
            {
                if (!view.IsVisibleOnView(position))
                    return default(KeyValuePair<IView, Vector2>);

                Vector2 windowPosition = view.GetPositionOnView(position) + view.BoundingBox.Origin + Resolution.Instance.WindowMargin;
                return new KeyValuePair<IView, Vector2>(view, windowPosition);
            }).Where(result => result.Key != null);
        }
    }
}