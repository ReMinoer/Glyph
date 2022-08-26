using System;

namespace Glyph.Scheduling.Base
{
    public class AsyncDelegateTaskBase<TAsyncDelegate, TDelegate> : DelegateTaskBase<TDelegate>
        where TAsyncDelegate : Delegate
        where TDelegate : Delegate
    {
        protected readonly TAsyncDelegate AsyncTaskDelegate;

        public AsyncDelegateTaskBase(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate)
            : base(taskDelegate)
        {
            AsyncTaskDelegate = asyncTaskDelegate;
        }
    }
}