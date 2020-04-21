using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using Simulacra.Binding;
using Simulacra.Binding.Collection;

namespace Glyph.Composition.Modelization
{
    static public class BindingCollectionExtension
    {
        static public IOneWayBinding<TModel, TView, NotifyCollectionChangedEventArgs> AddFactory<TModel, TView, TModelItem, TViewItem>(
            this CollectionBindingCollection<TModel, TView> bindingCollection,
            Expression<Func<TModel, IEnumerable<TModelItem>>> referenceGetterExpression,
            Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TModelItem : IGlyphCreator<TViewItem>
            where TViewItem : class, IGlyphComponent
        {
            return bindingCollection.AddFactoryBase(referenceGetterExpression, (x, m, v) => x, compositeGetter);
        }

        static public IOneWayBinding<TModel, TView, NotifyCollectionChangedEventArgs> AddFactory<TModel, TView, TModelItem>(
            this CollectionBindingCollection<TModel, TView> bindingCollection,
            Expression<Func<TModel, IEnumerable<TModelItem>>> referenceGetterExpression)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComposite<IGlyphComponent>
            where TModelItem : IGlyphCreator<IGlyphComponent>
        {
            return bindingCollection.AddFactoryBase(referenceGetterExpression, (x, m, v) => x, x => x);
        }

        static public IOneWayBinding<TModel, TView, NotifyCollectionChangedEventArgs> AddFactoryFromPaths<TModel, TView, TModelItem, TViewItem>(
            this CollectionBindingCollection<TModel, TView> bindingCollection,
            Expression<Func<TModel, IEnumerable<TModelItem>>> referenceGetterExpression,
            Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TModelItem : IAssetFile<IGlyphCreator<TViewItem>>
            where TViewItem : class, IGlyphComponent
        {
            return bindingCollection.AddFactoryBase(referenceGetterExpression, (x, m, v) => x.Data, compositeGetter);
        }

        static public IOneWayBinding<TModel, TView, NotifyCollectionChangedEventArgs> AddFactoryFromPaths<TModel, TView, TModelItem>(
            this CollectionBindingCollection<TModel, TView> bindingCollection,
            Expression<Func<TModel, IEnumerable<TModelItem>>> referenceGetterExpression)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComposite<IGlyphComponent>
            where TModelItem : IAssetFile<IGlyphCreator<IGlyphComponent>>
        {
            return bindingCollection.AddFactoryBase(referenceGetterExpression, (x, m, v) => x.Data, x => x);
        }

        static private IOneWayBinding<TModel, TView, NotifyCollectionChangedEventArgs> AddFactoryBase<TModel, TView, TModelItem, TViewItem>(
            this CollectionBindingCollection<TModel, TView> bindingCollection,
            Expression<Func<TModel, IEnumerable<TModelItem>>> referenceGetterExpression,
            Func<TModelItem, TModel, TView, IGlyphCreator<TViewItem>> creatorItemGetter,
            Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            where TModel : BindedData<TModel, TView>
            where TView : class, IGlyphComponent
            where TViewItem : class, IGlyphComponent
        {
            MemberExpression memberExpression = CollectionBindingCollectionExtension.GetMemberExpression(referenceGetterExpression);

            string modelPropertyName = memberExpression.Member.Name;
            Func<TModel, INotifyCollectionChanged> eventSourceGetter = Expression.Lambda<Func<TModel, INotifyCollectionChanged>>(memberExpression, referenceGetterExpression.Parameters).Compile();
            Func<TModel, IEnumerable<TModelItem>> referenceGetter = referenceGetterExpression.Compile();

            OneWayEventBinding<TModel, TView, NotifyCollectionChangedEventArgs, INotifyCollectionChanged> binding
                = new OneWayFactoryBinding<TModel,TView,TModelItem,TViewItem>(referenceGetter, creatorItemGetter, compositeGetter).AsEventBinding(eventSourceGetter);

            bindingCollection.Add(modelPropertyName, binding);
            return binding;
        }
    }
}