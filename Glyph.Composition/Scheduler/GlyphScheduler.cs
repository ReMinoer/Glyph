using System;
using System.Linq;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public class GlyphScheduler<TInterface, TDelegate> : Scheduler<TDelegate>, IGlyphScheduler<TInterface, TDelegate>
        where TInterface : IGlyphComponent
    {
        private readonly Func<TInterface, TDelegate> _interfaceToDelegate;

        public GlyphScheduler(Func<TInterface, TDelegate> interfaceToDelegate)
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
                throw new InvalidOperationException(typeof(TDelegate).Name + " is not a delegate type");

            _interfaceToDelegate = interfaceToDelegate;
        }

        new public IGlyphSchedulerController<TInterface, TDelegate> Plan(TDelegate item)
        {
            if (!DependencyGraph.ContainsItem(item))
                DependencyGraph.AddItem(item);

            return new GlyphSchedulerController(DependencyGraph, item, _interfaceToDelegate);
        }

        public IGlyphSchedulerController<TInterface, TDelegate> Plan(TInterface item)
        {
            return new GlyphSchedulerController(DependencyGraph, _interfaceToDelegate(item), _interfaceToDelegate);
        }

        public void Unplan(TInterface item)
        {
            DependencyGraph.ClearDependencies(_interfaceToDelegate(item));
        }

        internal void Add(TDelegate item)
        {
            if (!DependencyGraph.ContainsItem(item))
                DependencyGraph.AddItem(item);
        }

        internal void Remove(TDelegate item)
        {
            if (DependencyGraph.ContainsItem(item))
                DependencyGraph.RemoveItem(item);
        }

        internal void Clear()
        {
            DependencyGraph.ClearItems();
        }

        private class GlyphSchedulerController : SchedulerController, IGlyphSchedulerController<TInterface, TDelegate>
        {
            private readonly Func<TInterface, TDelegate> _interfaceToDelegate;

            public GlyphSchedulerController(IDependencyGraph<TDelegate> dependencyGraph, TDelegate item,
                Func<TInterface, TDelegate> interfaceToDelegate)
                : base(dependencyGraph, item)
            {
                _interfaceToDelegate = interfaceToDelegate;
            }

            public void Before(TInterface item)
            {
                Before(_interfaceToDelegate(item));
            }

            public void After(TInterface item)
            {
                After(_interfaceToDelegate(item));
            }

            public void Before<T>()
                where T : TInterface
            {
                foreach (TDelegate item in DependencyGraph.Select(x => x.Item))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && itemDelegate.Target is T)
                        Before(item);
                }
            }

            public void After<T>()
                where T : TInterface
            {
                foreach (TDelegate item in DependencyGraph.Select(x => x.Item))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && itemDelegate.Target is T)
                        After(item);
                }
            }

            public void Before(Type type)
            {
                foreach (TDelegate item in DependencyGraph.Select(x => x.Item))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        Before(item);
                }
            }

            public void After(Type type)
            {
                foreach (TDelegate item in DependencyGraph.Select(x => x.Item))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        After(item);
                }
            }
        }
    }
}