using System;
using Diese.Collections;

namespace Glyph
{
    public class ItemDisposedEventArgs : EventArgs
    {
        public INotifyDisposed Item { get; }
        public int Index { get; }

        public ItemDisposedEventArgs(INotifyDisposed item, int index)
        {
            Item = item;
            Index = index;
        }
    }

    public class DisposableTracker<T> : OrderedTrackerBase<T>
        where T : class, INotifyDisposed
    {
        public event EventHandler<ItemDisposedEventArgs> ItemDisposed;

        protected override sealed bool CanRegister(T item) => !item.IsDisposed;
        protected override sealed void Subscribe(T item) => item.Disposed += UnregisterHandler;
        protected override sealed void Unsubscribe(T item) => item.Disposed -= UnregisterHandler;

        protected void UnregisterHandler(object sender, EventArgs e)
        {
            var item = (T)sender;
            int index = IndexOf(item);

            Unregister(item);
            ItemDisposed?.Invoke(sender, new ItemDisposedEventArgs(item, index));
        }
    }
}