﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Diese.Collections;
using Simulacra.Binding.Collection.Base;

namespace Glyph.Composition.Modelization
{
    public class OneWayFactoryBinding<TModel, TView, TModelItem, TViewItem> : OneWayCollectionBindingBase<TModel, TView, TModelItem, TViewItem>
        where TModel : IGlyphData, IHierarchicalData
        where TModelItem: class, IGlyphData
        where TViewItem : class, IGlyphComponent
    {
        private readonly Func<TModel, TModelItem, TView, IGlyphCreator<TViewItem>> _creatorItemGetter;
        private readonly Func<TView, IGlyphComposite<TViewItem>> _compositeGetter;
        private readonly string _propertyName;

        public OneWayFactoryBinding(Func<TModel, IEnumerable<TModelItem>> referenceGetter,
            Func<TModel, TModelItem, TView, IGlyphCreator<TViewItem>> creatorItemGetter,
            Func<TView, IGlyphComposite<TViewItem>> compositeGetter, 
            string propertyName)
            : base(referenceGetter)
        {
            _creatorItemGetter = creatorItemGetter;
            _compositeGetter = compositeGetter;
            _propertyName = propertyName;
        }

        public override void SetView(TModel model, TView view)
        {
            base.SetView(model, view);

            if (!model.NotBinding)
                model.SubDataSources.Add(new ChildrenSource(_propertyName, _referenceGetter(model)));
        }

        protected override void ResetView(TModel model, TView view)
        {
            if (!model.NotBinding)
                model.SubDataSources.Remove(x => x.PropertyName == _propertyName);

            base.ResetView(model, view);
        }

        protected override void AddViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
            _compositeGetter(view).Add(viewItem);
        }

        protected override void RemoveViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem)
        {
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

        public class ChildrenSource : IGlyphDataChildrenSource
        {
            private readonly IList _list;
            public string PropertyName { get; }
            public IEnumerable<IGlyphData> Children { get; }
            public INotifyCollectionChanged ChildrenNotifier { get; }

            public ChildrenSource(string propertyName,
                IEnumerable<IGlyphData> children)
            {
                PropertyName = propertyName;
                Children = children;
                _list = children as IList;
                ChildrenNotifier = children as INotifyCollectionChanged;
            }

            public int IndexOf(IGlyphData data) => _list?.IndexOf(data) ?? -1;
            public void Set(int index, IGlyphData data) => _list?.Insert(index, data);
            public void Unset(int index) => _list?.RemoveAt(index);
        }
    }
}