namespace Glyph.Binding
{
    public interface ITwoWayBinding<in TModel, in TView> : IOneWayBinding<TModel, TView>
    {
        void UpdateModel(TModel model, TView view);
    }
}