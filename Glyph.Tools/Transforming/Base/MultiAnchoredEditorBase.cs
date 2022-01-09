using System.Collections.Generic;
using System.Linq;
using Glyph.Core;
using Glyph.Tools.Base;

namespace Glyph.Tools.Transforming.Base
{
    public class MultiAnchoredEditorBase<TController> : IntegratedEditorBase<TController>
        where TController : IAnchoredController
    {
        private List<AnchoredEditorBase<TController>> _editors;
        public override IEnumerable<IHandle> Handles => _editors.SelectMany(x => x.Handles);

        public MultiAnchoredEditorBase(GlyphResolveContext context)
            : base(context)
        {
            _editors = new List<AnchoredEditorBase<TController>>();
        }

        protected void AddEditor(AnchoredEditorBase<TController> editor)
        {
            _editors.Add(editor);
        }

        protected override void AssignEditedObject(TController editedObject)
        {
            foreach (AnchoredEditorBase<TController> editor in _editors)
                editor.EditedObject = editedObject;
        }
    }
}