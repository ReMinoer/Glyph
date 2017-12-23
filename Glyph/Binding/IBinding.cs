using System;

namespace Glyph.Binding
{
    public interface IOneSideBinding<in TModel, in TView>
    {
        void UpdateView(TModel model, TView view);
    }

    public interface ITwoWayBinding<in TModel, in TView> : IOneSideBinding<TModel, TView>
    {
        void UpdateModel(TModel model, TView view);
    }
}