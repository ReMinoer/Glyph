using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Diese.Collections;

namespace Glyph.Binding
{
    public class OneWayBinding<TProperty, TModel, TView> : IOneWayBinding<TModel, TView>
    {
        public Func<TModel, TProperty> ModelGetter { get; }
        public Action<TView, TProperty> ViewSetter { get; }

        public OneWayBinding(Func<TModel, TProperty> modelGetter, Action<TView, TProperty> viewSetter)
        {
            ModelGetter = modelGetter;
            ViewSetter = viewSetter;
        }
        
        public void UpdateView(TModel model, TView view) => ViewSetter(view, ModelGetter(model));
    }

    public class TwoWayBinding<TProperty, TModel, TView> : OneWayBinding<TProperty, TModel, TView>, ITwoWayBinding<TModel, TView>
    {
        public Action<TModel, TProperty> ModelSetter { get; }
        public Func<TView, TProperty> ViewGetter { get; }

        public TwoWayBinding(Expression<Func<TModel, TProperty>> modelGetter, Expression<Func<TView, TProperty>> viewGetter)
            : base(modelGetter.Compile(), viewGetter.ToSetter().Compile())
        {
            ModelSetter = modelGetter.ToSetter().Compile();
            ViewGetter = viewGetter.Compile();
        }

        public void UpdateModel(TModel model, TView view) => ModelSetter(model, ViewGetter(view));
    }

    public class OneWayBinding<TModelProperty, TViewProperty, TModel, TView> : IOneWayBinding<TModel, TView>
    {
        public Func<TModel, TModelProperty> ModelGetter { get; }
        public Action<TView, TViewProperty> ViewSetter { get; }
        public Func<TModelProperty, TModel, TView, TViewProperty> ModelToView { get; }

        public OneWayBinding(Func<TModel, TModelProperty> modelGetter, Action<TView, TViewProperty> viewSetter, Func<TModelProperty, TModel, TView, TViewProperty> modelToView)
        {
            ModelGetter = modelGetter;
            ViewSetter = viewSetter;
            ModelToView = modelToView;
        }
        
        public void UpdateView(TModel model, TView view) => ViewSetter(view, ModelToView(ModelGetter(model), model, view));
    }

    public class TwoWayBinding<TModelProperty, TViewProperty, TModel, TView> : OneWayBinding<TModelProperty, TViewProperty, TModel, TView>, ITwoWayBinding<TModel, TView>
    {
        public Action<TModel, TModelProperty> ModelSetter { get; }
        public Func<TView, TViewProperty> ViewGetter { get; }
        public Func<TViewProperty, TModel, TView, TModelProperty> ViewToModel { get; }

        public TwoWayBinding(Expression<Func<TModel, TModelProperty>> modelGetter, Expression<Func<TView, TViewProperty>> viewGetter, Func<TModelProperty, TModel, TView, TViewProperty> modelToView, Func<TViewProperty, TModel, TView, TModelProperty> viewToModel)
            : base(modelGetter.Compile(), viewGetter.ToSetter().Compile(), modelToView)
        {
            ModelSetter = modelGetter.ToSetter().Compile();
            ViewGetter = viewGetter.Compile();
            ViewToModel = viewToModel;
        }

        public void UpdateModel(TModel model, TView view) => ModelSetter(model, ViewToModel(ViewGetter(view), model, view));
    }

    public class OneWayCollectionBinding<TModelItem, TViewEnumerableItem, TViewCollectionItem, TModel, TView> : IOneWayBinding<TModel, TView>
        where TViewEnumerableItem : TViewCollectionItem
    {
        public Func<TModel, IEnumerable<TModelItem>> ModelEnumerableGetter { get; }
        public Func<TView, IEnumerable<TModelItem>, IEnumerable<TViewEnumerableItem>> ViewEnumerableGetter { get; }
        public Action<TView, TViewCollectionItem> ViewCollectionAdd { get; }
        public Action<TView, TViewCollectionItem> ViewCollectionRemove { get; }
        public Func<TModelItem, TModel, TView, TViewEnumerableItem> ModelToView { get; }

        public OneWayCollectionBinding(Func<TModel, IEnumerable<TModelItem>> modelEnumerableGetter,
                                       Func<TView, IEnumerable<TModelItem>, IEnumerable<TViewEnumerableItem>> viewEnumerableGetter,
                                       Func<TView, ICollection<TViewCollectionItem>> viewCollectionGetter,
                                       Func<TModelItem, TModel, TView, TViewEnumerableItem> modelToView)
            : this(modelEnumerableGetter, viewEnumerableGetter, (v, i) => viewCollectionGetter(v).Add(i), (v, i) => viewCollectionGetter(v).Remove(i), modelToView)
        {
        }

        public OneWayCollectionBinding(Func<TModel, IEnumerable<TModelItem>> modelEnumerableGetter,
                                       Func<TView, IEnumerable<TModelItem>, IEnumerable<TViewEnumerableItem>> viewEnumerableGetter,
                                       Action<TView, TViewCollectionItem> viewCollectionAdd,
                                       Action<TView, TViewCollectionItem> viewCollectionRemove,
                                       Func<TModelItem, TModel, TView, TViewEnumerableItem> modelToView)
        {
            ModelEnumerableGetter = modelEnumerableGetter;
            ViewEnumerableGetter = viewEnumerableGetter;
            ViewCollectionAdd = viewCollectionAdd;
            ViewCollectionRemove = viewCollectionRemove;
            ModelToView = modelToView;
        }

        public void UpdateView(TModel model, TView view)
        {
            List<TModelItem> modelEnumerable = ModelEnumerableGetter(model).ToList();
            IEnumerable<TViewEnumerableItem> modelConvertedEnumerable = modelEnumerable.Select(x => ModelToView(x, model, view));
            IEnumerable<TViewEnumerableItem> viewEnumerable = ViewEnumerableGetter(view, modelEnumerable);

            if (!viewEnumerable.SetDiff(modelConvertedEnumerable, out IEnumerable<TViewEnumerableItem> added, out IEnumerable<TViewEnumerableItem> removed))
                return;

            foreach (TViewEnumerableItem toRemove in removed)
                ViewCollectionRemove(view, toRemove);
            foreach (TViewEnumerableItem toAdd in added)
                ViewCollectionAdd(view, toAdd);
        }
    }

    public class TwoWayCollectionBinding<TModelEnumerableItem, TModelCollectionItem, TViewEnumerableItem, TViewCollectionItem, TModel, TView> : OneWayCollectionBinding<TModelEnumerableItem, TViewEnumerableItem, TViewCollectionItem, TModel, TView>, ITwoWayBinding<TModel, TView>
        where TViewEnumerableItem : TViewCollectionItem
        where TModelEnumerableItem : TModelCollectionItem
    {
        public Action<TView, TModelEnumerableItem> ModelCollectionAdd { get; }
        public Action<TView, TModelEnumerableItem> ModelCollectionRemove { get; }
        public Func<TViewEnumerableItem, TModel, TView, TModelEnumerableItem> ViewToModel { get; }

        public TwoWayCollectionBinding(Func<TModel, IEnumerable<TModelEnumerableItem>> modelEnumerableGetter,
                                       Func<TView, ICollection<TModelCollectionItem>> modelCollectionGetter,
                                       Func<TView, IEnumerable<TModelEnumerableItem>, IEnumerable<TViewEnumerableItem>> viewEnumerableGetter,
                                       Func<TView, ICollection<TViewCollectionItem>> viewCollectionGetter,
                                       Func<TModelEnumerableItem, TModel, TView, TViewEnumerableItem> modelToView,
                                       Func<TViewEnumerableItem, TModel, TView, TModelEnumerableItem> viewToModel)
            : this(modelEnumerableGetter, (m, i) => modelCollectionGetter(m).Add(i), (m, i) => modelCollectionGetter(m).Remove(i), viewEnumerableGetter, (v, i) => viewCollectionGetter(v).Add(i), (v, i) => viewCollectionGetter(v).Remove(i), modelToView, viewToModel)
        {
        }

        public TwoWayCollectionBinding(Func<TModel, IEnumerable<TModelEnumerableItem>> modelEnumerableGetter,
                                       Action<TView, TModelEnumerableItem> modelCollectionAdd,
                                       Action<TView, TModelEnumerableItem> modelCollectionRemove,
                                       Func<TView, IEnumerable<TModelEnumerableItem>, IEnumerable<TViewEnumerableItem>> viewEnumerableGetter,
                                       Action<TView, TViewCollectionItem> viewCollectionAdd,
                                       Action<TView, TViewCollectionItem> viewCollectionRemove,
                                       Func<TModelEnumerableItem, TModel, TView, TViewEnumerableItem> modelToView,
                                       Func<TViewEnumerableItem, TModel, TView, TModelEnumerableItem> viewToModel)
            : base(modelEnumerableGetter, viewEnumerableGetter, viewCollectionAdd, viewCollectionRemove, modelToView)
        {
            ModelCollectionAdd = modelCollectionAdd;
            ModelCollectionRemove = modelCollectionRemove;
            ViewToModel = viewToModel;
        }

        public void UpdateModel(TModel model, TView view)
        {
            List<TModelEnumerableItem> modelEnumerable = ModelEnumerableGetter(model).ToList();
            IEnumerable<TModelEnumerableItem> viewConvertedEnumerable = ViewEnumerableGetter(view, modelEnumerable).Select(x => ViewToModel(x, model, view));

            if (!modelEnumerable.SetDiff(viewConvertedEnumerable, out IEnumerable<TModelEnumerableItem> added, out IEnumerable<TModelEnumerableItem> removed))
                return;

            foreach (TModelEnumerableItem toRemove in removed)
                ModelCollectionRemove(view, toRemove);
            foreach (TModelEnumerableItem toAdd in added)
                ModelCollectionAdd(view, toAdd);
        }
    }
}