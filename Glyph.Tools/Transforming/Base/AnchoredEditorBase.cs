using System.Collections.Generic;
using Glyph.Core;
using Glyph.Tools.Base;
using Glyph.UI;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class AnchoredEditorBase<TController> : GlyphObject, IIntegratedEditor<TController>
        where TController : IAnchoredController
    {
        protected readonly AnchoredSceneNode AnchoredSceneNode;

        protected abstract IEnumerable<IHandle> Handles { get; }

        private TController _editedObject;
        public virtual TController EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                AnchoredSceneNode.AnchorNode = _editedObject.Anchor;

                AssignEditedObjectToHandles(_editedObject);
            }
        }

        object IIntegratedEditor.EditedObject => EditedObject;
        protected abstract void AssignEditedObjectToHandles(TController editedObject);

        private IDrawClient _raycastClient;
        public IDrawClient RaycastClient
        {
            get => _raycastClient;
            set
            {
                _raycastClient = value;

                foreach (IHandle handle in Handles)
                    handle.RaycastClient = _raycastClient;
            }
        }
        
        public ISceneNode ParentNode
        {
            get => AnchoredSceneNode.ParentNode;
            set => AnchoredSceneNode.SetParent(value);
        }

        protected AnchoredEditorBase(GlyphResolveContext context)
            : base(context)
        {
            AnchoredSceneNode = Add<AnchoredSceneNode>();
            AnchoredSceneNode.IgnoreRotation = true;
            AnchoredSceneNode.IgnoreScale = true;
            AnchoredSceneNode.ProjectionConfiguration = x => x.WithViewDepthMax(1);

            Add<UserInterface>();
        }
    }
}