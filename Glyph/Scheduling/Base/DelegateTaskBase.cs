using System;

namespace Glyph.Scheduling.Base
{
    public class DelegateTaskBase<TDelegate>
        where TDelegate : Delegate
    {
        protected readonly TDelegate TaskDelegate;
        public DelegateTaskBase(TDelegate taskDelegate) => TaskDelegate = taskDelegate;

        public override string ToString() => $"{TaskDelegate.Method.DeclaringType?.Name}.{TaskDelegate.Method.Name} - {TaskDelegate.Target}";
    }
}