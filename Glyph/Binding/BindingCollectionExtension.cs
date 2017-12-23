using System;
using System.Linq.Expressions;

namespace Glyph.Binding
{
    static public class BindingCollectionExtension
    {
        static public IOneSideBinding<TModel, TView> Add<TModel, TView, TProperty>(this OneSideBindingCollection<TModel, TView> bindingCollection, Expression<Func<TModel, TProperty>> modelGetter, Expression<Func<TView, TProperty>> viewGetter)
        {
            var binding = new Binding<TProperty, TModel, TView>(modelGetter, viewGetter);
            bindingCollection.Add(binding, ((MemberExpression)modelGetter.Body).Member.Name);
            return binding;
        }

        static public IOneSideBinding<TModel, TView> Add<TModel, TView, TModelProperty, TViewProperty>(this OneSideBindingCollection<TModel, TView> bindingCollection, Expression<Func<TModel, TModelProperty>> modelGetter, Expression<Func<TView, TViewProperty>> viewGetter, Func<TModelProperty, TModel, TView, TViewProperty> modelToView, Func<TViewProperty, TModel, TView, TModelProperty> viewToModel)
        {
            var binding = new Binding<TModelProperty, TViewProperty, TModel, TView>(modelGetter, viewGetter, modelToView, viewToModel);
            bindingCollection.Add(binding, ((MemberExpression)modelGetter.Body).Member.Name);
            return binding;
        }
    }
}