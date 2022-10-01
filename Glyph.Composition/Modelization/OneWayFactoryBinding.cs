using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Diese.Collections;
using Diese.Collections.Observables;
using Simulacra.Binding.Collection.Base;

namespace Glyph.Composition.Modelization
{
    public class OneWayFactoryBinding<TModel, TView, TModelItem, TViewItem> : OneWayListBindingBase<TModel, TView, TModelItem, TViewItem>
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
            _compositeGetter(view).CreateSubComposite(_propertyName);

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

            _compositeGetter(view).RemoveSubComposite(_propertyName);
        }

        private IList<TViewItem> GetSubComposite(TView view) => _compositeGetter(view).GetSubComposite(_propertyName);

        protected override void AddViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem) => GetSubComposite(view).Add(viewItem);
        protected override void RemoveViewItem(TView view, TViewItem viewItem, TModel model, TModelItem modelItem) => GetSubComposite(view).Remove(viewItem);
        protected override void DisposeViewItem(TView view, TViewItem viewItem) => viewItem.Dispose();

        protected override TViewItem GetBindedViewItem(TView view, TModel model, TModelItem modelItem) => _creatorItemGetter(model, modelItem, view).BindedObject;
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

        protected override void InsertViewItems(TView view, IEnumerable<TViewItem> viewItems, TModel model, IEnumerable<TModelItem> modelItems, int index)
        {
            IList<TViewItem> composite = GetSubComposite(view);

            foreach (TViewItem item in viewItems)
            {
                composite.Insert(index, item);
                index++;
            }
        }

        protected override void ReplaceViewItems(TView view, IEnumerable<TViewItem> viewItems, TModel model, IEnumerable<TModelItem> modelItems, int index)
        {
            IList<TViewItem> composite = GetSubComposite(view);

            foreach (TViewItem item in viewItems)
            {
                composite[index] = item;
                index++;
            }
        }

        protected override void MoveViewItems(TView view, IEnumerable<TViewItem> viewItems, TModel model, IEnumerable<TModelItem> modelItems, int index)
        {
            IList<TViewItem> composite = GetSubComposite(view);

            TViewItem[] itemsArray = viewItems.ToArray();
            List<int> oldIndices = itemsArray.Select(composite.IndexOf).OrderByDescending(x => x).ToList();

            foreach (int oldIndex in oldIndices)
                composite.RemoveAt(oldIndex);

            foreach (TViewItem item in itemsArray)
            {
                composite.Insert(index, item);
                index++;
            }
        }

        public class ChildrenSource : IGlyphDataChildrenSource
        {
            private readonly IList _list;
            private readonly IObservableList _observableList;
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
                _observableList = children as IObservableList;
                ChildrenNotifier = children as INotifyCollectionChanged;

                _itemType = children.GetType()
                    .GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                    .GetGenericArguments()[0];
            }

            public int IndexOf(IGlyphData data) => _list?.IndexOf(data) ?? -1;

            public bool CanInsert(int index) => index >= 0 && index <= Count && (_list is null || !_list.IsFixedSize);
            public bool CanInsert(int index, IGlyphData data) => CanInsert(index) && (_itemType is null || _itemType.IsInstanceOfType(data));
            public bool CanRemoveAt(int index) => index >= 0 && index < Count && (_list is null || !_list.IsFixedSize);
            public bool CanMove(int oldIndex, int newIndex) => oldIndex >= 0 && oldIndex < Count && newIndex >= 0 && newIndex < Count && (_list is null || !_list.IsFixedSize);

            public void Insert(int index, IGlyphData data) => _list?.Insert(index, data);
            public void RemoveAt(int index) => _list?.RemoveAt(index);
            public void Move(int oldIndex, int newIndex)
            {
                if (_observableList != null)
                {
                    _observableList.Move(oldIndex, newIndex);
                }
                else if (_list != null)
                {
                    object data = _list[oldIndex];
                    _list.RemoveAt(oldIndex);
                    _list.Insert(newIndex, data);
                }
            }
        }
    }
}