using System;
using System.Collections.Generic;
using Simulacra.Binding.Collection.Base;

namespace Glyph.Composition.Modelization
{
    public class OneWayFactoryBinding<TModel, TView, TModelItem, TViewItem> : OneWayCollectionBindingBase<TModel, TView, TModelItem, TViewItem>
        where TModel : BindedData<TModel, TView>
        where TView : class, IGlyphComponent
        where TModelItem : IGlyphCreator<TViewItem>
        where TViewItem : class, IGlyphComponent
    {
        private readonly Func<TView, IGlyphComposite<TViewItem>> _compositeGetter;

        public OneWayFactoryBinding(Func<TModel, IEnumerable<TModelItem>> referenceGetter, Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            : base(referenceGetter)
        {
            _compositeGetter = compositeGetter;
        }

        protected override void AddViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            _compositeGetter(view).Add(viewItem);
            model.SubData.Add(modelItem);
        }

        protected override void RemoveViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            model.SubData.Remove(modelItem);
            _compositeGetter(view).Remove(viewItem);
        }

        protected override void DisposeViewItem(TView view, TViewItem viewItem)
        {
            viewItem.Dispose();
        }

        protected override TViewItem CreateBindedViewItem(TView view, TModel model, TModelItem modelItem)
        {
            modelItem.DependencyResolver = model.Resolver;
            modelItem.Instantiate();
            return modelItem.BindedObject;
        }

        protected override TViewItem GetBindedViewItem(TView view, TModelItem modelItem)
        {
            return modelItem.BindedObject;
        }
    }
}