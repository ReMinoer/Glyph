using System;
using System.Linq.Expressions;

namespace Glyph.Binding
{
    public class Binding<TProperty, TModel, TView> : ITwoWayBinding<TModel, TView>
    {
        public Func<TModel, TProperty> ModelGetter { get; private set; }
        public Action<TModel, TProperty> ModelSetter { get; private set; }
        public Func<TView, TProperty> ViewGetter { get; private set; }
        public Action<TView, TProperty> ViewSetter { get; private set; }

        public Binding(Expression<Func<TModel, TProperty>> modelGetter, Expression<Func<TView, TProperty>> viewGetter)
        {
            ModelGetter = modelGetter.Compile();
            ModelSetter = modelGetter.ToSetter().Compile();
            ViewGetter = viewGetter.Compile();
            ViewSetter = viewGetter.ToSetter().Compile();
        }

        public void UpdateModel(TModel model, TView view) => ModelSetter(model, ViewGetter(view));
        public void UpdateView(TModel model, TView view) => ViewSetter(view, ModelGetter(model));
    }

    public class Binding<TModelProperty, TViewProperty, TModel, TView> : ITwoWayBinding<TModel, TView>
    {
        public Func<TModel, TModelProperty> ModelGetter { get; private set; }
        public Action<TModel, TModelProperty> ModelSetter { get; private set; }
        public Func<TView, TViewProperty> ViewGetter { get; private set; }
        public Action<TView, TViewProperty> ViewSetter { get; private set; }
        
        public Func<TModelProperty, TModel, TView, TViewProperty> ModelToView { get; private set; }
        public Func<TViewProperty, TModel, TView, TModelProperty> ViewToModel  { get; private set; }

        public Binding(Expression<Func<TModel, TModelProperty>> modelGetter, Expression<Func<TView, TViewProperty>> viewGetter, Func<TModelProperty, TModel, TView, TViewProperty> modelToView, Func<TViewProperty, TModel, TView, TModelProperty> viewToModel)
        {
            ModelGetter = modelGetter.Compile();
            ModelSetter = modelGetter.ToSetter().Compile();
            ViewGetter = viewGetter.Compile();
            ViewSetter = viewGetter.ToSetter().Compile();

            ModelToView = modelToView;
            ViewToModel = viewToModel;
        }

        public void UpdateModel(TModel model, TView view) => ModelSetter(model, ViewToModel(ViewGetter(view), model, view));
        public void UpdateView(TModel model, TView view) => ViewSetter(view, ModelToView(ModelGetter(model), model, view));
    }
}