using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
            {
                if (!model.SubDataSources.Any(x => x is ChildrenSource childrenSource && childrenSource.PropertyName == _propertyName))
                    model.SubDataSources.Add(new ChildrenSource(model, _propertyName, _referenceGetter(model)));
            }
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
            private readonly Type _itemType;

            public IGlyphData Owner { get; }
            public int Count => _list?.Count ?? Children.Count();
            public bool HasData => true;

            public string PropertyName { get; }
            public IEnumerable<IGlyphData> Children { get; }
            public INotifyCollectionChanged ChildrenNotifier { get; }

            public ChildrenSource(IGlyphData owner,
                string propertyName,
                IEnumerable<IGlyphData> children)
            {
                Owner = owner;
                PropertyName = propertyName;
                Children = children;

                _list = children as IList;
                ChildrenNotifier = children as INotifyCollectionChanged;

                _itemType = children.GetType()
                    .GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                    .GetGenericArguments()[0];
            }

            public int IndexOf(IGlyphData data) => _list?.IndexOf(data) ?? -1;

            public bool CanInsert(int index, IGlyphData data) => index >= 0 && index <= Count
                && (_list is null || !_list.IsFixedSize)
                && (_itemType is null || _itemType.IsInstanceOfType(data));

            public bool CanRemoveAt(int index) => index >= 0 && index <= Count
                && (_list is null || !_list.IsFixedSize);

            public void Insert(int index, IGlyphData data) => _list?.Insert(index, data);
            public void RemoveAt(int index) => _list?.RemoveAt(index);
        }
    }
}