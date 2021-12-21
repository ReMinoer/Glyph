using System.Collections.Generic;
using Glyph.Core;
using Glyph.Tools.Base;
using Glyph.UI;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class IntegratedEditorBase<TController> : GlyphObject, IIntegratedEditor<TController>
    {
        protected abstract IEnumerable<IHandle> Handles { get; }

        private TController _editedObject;
        public virtual TController EditedObject
        {
            get => _editedObject;
            set
            {
                _editedObject = value;
                AssignEditedObject(_editedObject);
            }
        }

        object IIntegratedEditor.EditedObject => EditedObject;
        protected abstract void AssignEditedObject(TController editedObject);

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

        protected IntegratedEditorBase(GlyphResolveContext context)
            : base(context)
        {
            Add<UserInterface>();
        }
    }
}