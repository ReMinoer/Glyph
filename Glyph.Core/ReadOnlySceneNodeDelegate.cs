using System;
using Glyph.Core.Base;

namespace Glyph.Core
{
    public class ReadOnlySceneNodeDelegate : ReadOnlySceneNodeBase
    {
        private readonly Func<ISceneNode> _sceneNodeFunc;
        protected override ISceneNode SceneNode => _sceneNodeFunc();

        public ReadOnlySceneNodeDelegate(Func<ISceneNode> sceneNodeFunc)
        {
            _sceneNodeFunc = sceneNodeFunc;
        }
    }
}