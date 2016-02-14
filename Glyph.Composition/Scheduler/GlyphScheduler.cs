using System;
using System.Linq;
using Diese.Injection;
using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public class GlyphScheduler<TInterface, TDelegate> : Scheduler<TDelegate>, IGlyphScheduler<TInterface, TDelegate>
        where TInterface : class, IGlyphComponent
    {
        private readonly Func<TInterface, TDelegate> _interfaceToDelegate;
        private readonly Func<GlyphObject, TDelegate> _glyphObjectToDelegate;

        public GlyphScheduler(IDependencyInjector injector, Func<TInterface, TDelegate> interfaceToDelegate, Func<GlyphObject, TDelegate> glyphObjectToDelegate)
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
                throw new InvalidOperationException(typeof(TDelegate).Name + " is not a delegate type");

            _interfaceToDelegate = interfaceToDelegate;
            _glyphObjectToDelegate = glyphObjectToDelegate;

            ApplyProfile(injector.Resolve<SchedulerProfile<TInterface>>());
        }

        new public IGlyphSchedulerController<TInterface, TDelegate> Plan(TDelegate item)
        {
            base.Plan(item);

            return new GlyphSchedulerController(this, ItemsVertex[item], _interfaceToDelegate);
        }

        public IGlyphSchedulerController<TInterface, TDelegate> Plan(TInterface item)
        {
            return Plan(_interfaceToDelegate(item));
        }

        public void Unplan(TInterface item)
        {
            Unplan(_interfaceToDelegate(item));
        }

        void IGlyphSchedulerAssigner.AssignComponent(IGlyphComponent component)
        {
            var castedComponent = component as TInterface;
            if (castedComponent != null)
                Add(_interfaceToDelegate(castedComponent));
        }

        void IGlyphSchedulerAssigner.AssignComponent(GlyphObject glyphObject)
        {
            Add(_glyphObjectToDelegate(glyphObject));
        }

        void IGlyphSchedulerAssigner.RemoveComponent(IGlyphComponent component)
        {
            var castedComponent = component as TInterface;
            if (castedComponent != null)
                Remove(_interfaceToDelegate(castedComponent));
        }

        void IGlyphSchedulerAssigner.RemoveComponent(GlyphObject glyphObject)
        {
            Remove(_glyphObjectToDelegate(glyphObject));
        }

        void IGlyphSchedulerAssigner.ClearComponents()
        {
            Clear();
        }

        private sealed class GlyphSchedulerController : SchedulerController, IGlyphSchedulerController<TInterface, TDelegate>
        {
            private readonly Func<TInterface, TDelegate> _interfaceToDelegate;

            public GlyphSchedulerController(Scheduler<TDelegate> scheduler,
                SchedulerGraph<TDelegate>.Vertex vertex,
                Func<TInterface, TDelegate> interfaceToDelegate)
                : base(scheduler, vertex)
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
                foreach (TDelegate item in SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && itemDelegate.Target is T)
                        Before(item);
                }
            }

            public void After<T>()
                where T : TInterface
            {
                foreach (TDelegate item in SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && itemDelegate.Target is T)
                        After(item);
                }
            }

            public void Before(Type type)
            {
                foreach (TDelegate item in SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        Before(item);
                }
            }

            public void After(Type type)
            {
                foreach (TDelegate item in SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        After(item);
                }
            }
        }
    }
}