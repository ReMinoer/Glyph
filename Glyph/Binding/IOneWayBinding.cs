using System;

namespace Glyph.Binding
{
    public interface IOneWayBinding<in TModel, in TView>
    {
        void UpdateView(TModel model, TView view);
    }
}