using Glyph.Core;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class AnchoredEditorBase<TController> : IntegratedEditorBase<TController>
        where TController : IAnchoredController
    {
        protected readonly AnchoredSceneNode AnchoredSceneNode;

        protected override void AssignEditedObject(TController editedObject)
        {
            AnchoredSceneNode.AnchorNode = editedObject.Anchor;
            AssignEditedObjectToHandles(editedObject);
        }

        protected abstract void AssignEditedObjectToHandles(TController editedObject);
        
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
        }
    }
}