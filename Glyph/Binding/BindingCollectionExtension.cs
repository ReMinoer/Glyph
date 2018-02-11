using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Glyph.Binding
{
    static public class BindingCollectionExtension
    {
        static public IOneWayBinding<TModel, TView> Add<TModel, TView, TProperty>(this OneSideBindingCollection<TModel, TView> bindingCollection,
                                                                                  Expression<Func<TModel, TProperty>> modelGetter,
                                                                                  Expression<Func<TView, TProperty>> viewGetter)
        {
            var binding = new OneWayBinding<TProperty, TModel, TView>(modelGetter.Compile(), viewGetter.ToSetter().Compile());
            bindingCollection.Add(binding, ((MemberExpression)modelGetter.Body).Member.Name);
            return binding;
        }

        static public IOneWayBinding<TModel, TView> Add<TModel, TView, TModelProperty, TViewProperty>(this OneSideBindingCollection<TModel, TView> bindingCollection,
                                                                                                      Expression<Func<TModel, TModelProperty>> modelGetter,
                                                                                                      Expression<Func<TView, TViewProperty>> viewGetter,
                                                                                                      Func<TModelProperty, TModel, TView, TViewProperty> modelToView)
        {
            var binding = new OneWayBinding<TModelProperty, TViewProperty, TModel, TView>(modelGetter.Compile(), viewGetter.ToSetter().Compile(), modelToView);
            bindingCollection.Add(binding, ((MemberExpression)modelGetter.Body).Member.Name);
            return binding;
        }

        static public IOneWayBinding<TModel, TView> AddCollection<TModelItem, TViewEnumerableItem, TViewCollectionItem, TModel, TView>(this OneSideBindingCollection<TModel, TView> bindingCollection,
                                                                                                                                       Expression<Func<TModel, IEnumerable<TModelItem>>> modelGetter,
                                                                                                                                       Func<TView, IEnumerable<TModelItem>, IEnumerable<TViewEnumerableItem>> viewEnumerableGetter,
                                                                                                                                       Func<TView, ICollection<TViewCollectionItem>> viewCollectionGetter,
                                                                                                                                       Func<TModelItem, TModel, TView, TViewEnumerableItem> modelToView)
            where TViewEnumerableItem : TViewCollectionItem
        {
            var binding = new OneWayCollectionBinding<TModelItem, TViewEnumerableItem, TViewCollectionItem, TModel, TView>(modelGetter.Compile(), viewEnumerableGetter, viewCollectionGetter, modelToView);
            bindingCollection.Add(binding, ((MemberExpression)modelGetter.Body).Member.Name);
            return binding;
        }
    }
}