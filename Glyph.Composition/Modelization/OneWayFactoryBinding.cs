using System;
using System.Collections.Generic;
using Simulacra.Binding.Collection.Base;

namespace Glyph.Composition.Modelization
{
    public class OneWayFactoryBinding<TModel, TView, TModelItem, TViewItem> : OneWayCollectionBindingBase<TModel, TView, TModelItem, TViewItem>
        where TModel : IGlyphData, IHierarchicalData
        where TViewItem : class, IGlyphComponent
    {
        private readonly Func<TModel, TModelItem, TView, IGlyphCreator<TViewItem>> _creatorItemGetter;
        private readonly Func<TView, IGlyphComposite<TViewItem>> _compositeGetter;

        public OneWayFactoryBinding(Func<TModel, IEnumerable<TModelItem>> referenceGetter, Func<TModel, TModelItem, TView, IGlyphCreator<TViewItem>> creatorItemGetter, Func<TView, IGlyphComposite<TViewItem>> compositeGetter)
            : base(referenceGetter)
        {
            _creatorItemGetter = creatorItemGetter;
            _compositeGetter = compositeGetter;
        }

        protected override void AddViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            _compositeGetter(view).Add(viewItem);
            model.SubData.Add(_creatorItemGetter(model, modelItem, view));
        }

        protected override void RemoveViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            model.SubData.Remove(_creatorItemGetter(model, modelItem, view));
            _compositeGetter(view).Remove(viewItem);
        }

        protected override void DisposeViewItem(TView view, TViewItem viewItem)
        {
            viewItem.Dispose();
        }

        protected override TViewItem CreateBindedViewItem(TView view, TModel model, TModelItem modelItem)
        {
            IGlyphCreator<TViewItem> creatorItem = _creatorItemGetter(model, modelItem, view);

            creatorItem.DependencyResolver = model.DependencyResolver;
            creatorItem.SerializationKnownTypes = model.SerializationKnownTypes;

            if (model.NotBinding)
                return creatorItem.Create();

            creatorItem.Instantiate();
            return creatorItem.BindedObject;
        }

        protected override TViewItem GetBindedViewItem(TView view, TModel model, TModelItem modelItem)
        {
            return _creatorItemGetter(model, modelItem, view).BindedObject;
        }
    }
}