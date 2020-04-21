using System;
using System.Collections.Generic;
using Simulacra.Binding.Collection.Base;

namespace Glyph.Composition.Modelization
{
    public class OneWayFactoryBinding<TModel, TView, TModelItem, TViewItem> : OneWayCollectionBindingBase<TModel, TView, TModelItem, TViewItem>
        where TModel : BindedData<TModel, TView>
        where TView : class, IGlyphComponent
        where TViewItem : class, IGlyphComponent
    {
        private readonly Func<TModelItem, TModel, TView, IGlyphCreator<TViewItem>> _creatorItemGetter;
        private readonly Func<TView, IGlyphComposite<TViewItem>> _compositeGetter;

        public OneWayFactoryBinding(Func<TModel, IEnumerable<TModelItem>> referenceGetter, Func<TModelItem, TModel, TView, IGlyphCreator<TViewItem>> creatorItemGetter, Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            : base(referenceGetter)
        {
            _creatorItemGetter = creatorItemGetter;
            _compositeGetter = compositeGetter;
        }

        protected override void AddViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            _compositeGetter(view).Add(viewItem);
            model.SubData.Add(_creatorItemGetter(modelItem, model, view));
        }

        protected override void RemoveViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            model.SubData.Remove(_creatorItemGetter(modelItem, model, view));
            _compositeGetter(view).Remove(viewItem);
        }

        protected override void DisposeViewItem(TView view, TViewItem viewItem)
        {
            viewItem.Dispose();
        }

        protected override TViewItem CreateBindedViewItem(TView view, TModel model, TModelItem modelItem)
        {
            IGlyphCreator<TViewItem> creatorItem = _creatorItemGetter(modelItem, model, view);

            creatorItem.DependencyResolver = model.Resolver;
            creatorItem.Instantiate();
            return creatorItem.BindedObject;
        }

        protected override TViewItem GetBindedViewItem(TView view, TModel model, TModelItem modelItem)
        {
            return _creatorItemGetter(modelItem, model, view).BindedObject;
        }
    }
}