using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class ViewManager : GlyphContainer, ILoadContent, IUpdate
    {
        static private ViewManager _main;
        static public ViewManager Main
        {
            get { return _main ?? (_main = new ViewManager()); }
        }

        private readonly SceneNode _sceneNode;
        private readonly List<IView> _views;
        private readonly IReadOnlyCollection<IView> _readOnlyScreens;

        public IEnumerable<IView> Views
        {
            get { return _readOnlyScreens; }
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

            foreach (IView view in Views)
                view.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (IView view in Views)
                view.LoadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (IView view in Views)
                view.Update(elapsedTime);
        }

        public void RegisterView(IView view)
        {
            Components.Add(view);
            _views.Add(view);

            view.Initialize();
        }

        public void UnregisterView(IView view)
        {
            _views.Remove(view);
        }

        public bool IsPointVisible(Vector2 position, IDrawClient drawClient = null)
        {
            return Views.Any(view => view.Displayed(drawClient) && view.IsVisibleOnView(position));
        }

        public bool IsPointVisible(Vector2 position, IDrawClient drawClient, out IView view)
        {
            foreach (IView v in Views.Where(v => v.Displayed(drawClient)))
            {
                if (!v.IsVisibleOnView(position))
                    continue;

                view = v;
                return true;
            }

            view = null;
            return false;
        }

        public IView GetViewAtPoint(Vector2 virtualScreenPosition, IDrawClient drawClient = null)
        {
            return Views.Reverse().FirstOrDefault(view => view.Displayed(drawClient) && view.BoundingBox.ContainsPoint(virtualScreenPosition));
        }

        public IView GetViewAtPoint(Vector2 virtualScreenPosition, IDrawClient drawClient, out Vector2 positionOnView)
        {
            IView view = GetViewAtPoint(virtualScreenPosition) ?? Views.FirstOrDefault(v => v.Displayed(drawClient));
            if (view == null)
            {
                positionOnView = Vector2.Zero;
                return null;
            }

            positionOnView = virtualScreenPosition - view.BoundingBox.Position;
            return view;
        }

        public Vector2 GetPositionOnVirtualScreen(Vector2 viewPosition, IView view)
        {
            return viewPosition + view.BoundingBox.Position;
        }
    }
}