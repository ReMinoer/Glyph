using System;
using System.Linq;
using Diese.Injection;
using Glyph.Composition;
using Glyph.Core.Scheduler.Base;
using Glyph.Core.Scheduler.Base.Controllers;

namespace Glyph.Core.Scheduler
{
    public class GlyphScheduler<TInterface, TDelegate> : SchedulerBase<GlyphScheduler<TInterface, TDelegate>.Controller, TDelegate>, IGlyphScheduler<GlyphScheduler<TInterface, TDelegate>.Controller, TInterface, TDelegate>
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

        new public Controller Plan(TDelegate item)
        {
            base.Plan(item);

            return CreateController(ItemsVertex[item]);
        }

        protected override Controller CreateController(SchedulerGraph<TDelegate>.Vertex vertex)
        {
            return new Controller(this, vertex, _interfaceToDelegate);
        }

        public Controller Plan(TInterface item)
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
        
        public class Controller : IGlyphSchedulerController<Controller, TInterface, TDelegate>
        {
            private readonly PriorityController<Controller, TDelegate> _priorityController;
            private readonly RelativeController<Controller, TDelegate> _relativeController;
            private readonly SchedulerBase<Controller, TDelegate> _scheduler;
            private readonly Func<TInterface, TDelegate> _interfaceToDelegate;

            public Controller(SchedulerBase<Controller, TDelegate> scheduler, SchedulerGraph<TDelegate>.Vertex vertex, Func<TInterface, TDelegate> interfaceToDelegate)
            {
                _priorityController = new PriorityController<Controller, TDelegate>(scheduler, vertex);
                _relativeController = new RelativeController<Controller, TDelegate>(scheduler, vertex);

                _scheduler = scheduler;
                _interfaceToDelegate = interfaceToDelegate;
            }

            public Controller Before(TInterface item)
            {
                Before(_interfaceToDelegate(item));
                return this;
            }

            public Controller After(TInterface item)
            {
                After(_interfaceToDelegate(item));
                return this;
            }

            public Controller Before<T>()
                where T : TInterface
            {
                foreach (TDelegate item in _scheduler.SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate?.Target is T)
                        Before(item);
                }
                return this;
            }

            public Controller After<T>()
                where T : TInterface
            {
                foreach (TDelegate item in _scheduler.SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate?.Target is T)
                        After(item);
                }
                return this;
            }

            public Controller Before(Type type)
            {
                foreach (TDelegate item in _scheduler.SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        Before(item);
                }
                return this;
            }

            public Controller After(Type type)
            {
                foreach (TDelegate item in _scheduler.SchedulerGraph.Vertices.SelectMany(x => x.Items))
                {
                    var itemDelegate = item as Delegate;
                    if (itemDelegate != null && type.IsInstanceOfType(itemDelegate.Target))
                        After(item);
                }
                return this;
            }

            public Controller After(TDelegate item)
            {
                _relativeController.After(item);
                return this;
            }

            public Controller Before(TDelegate item)
            {
                _relativeController.Before(item);
                return this;
            }

            public Controller AtEnd()
            {
                _priorityController.AtEnd();
                return this;
            }

            public Controller AtStart()
            {
                _priorityController.AtStart();
                return this;
            }

            void IRelativeController<TDelegate>.Before(TDelegate item)
            {
                _relativeController.Before(item);
            }

            void IRelativeController<TDelegate>.After(TDelegate item)
            {
                _relativeController.After(item);
            }

            void IPriorityController.AtStart()
            {
                _priorityController.AtStart();
            }

            void IPriorityController.AtEnd()
            {
                _priorityController.AtEnd();
            }
        }
    }
}