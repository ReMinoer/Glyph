using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Composition.Utils;
using Diese.Debug;
using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Injection;
using Glyph.Composition.Messaging;
using Glyph.Composition.Scheduler;
using Glyph.Messaging;

namespace Glyph.Composition
{
    public abstract class GlyphSchedulableBase : GlyphComposite, IEnableable, ILoadContent, IUpdate, IDraw
    {
        static public readonly WatchTree UpdateWatchTree = new WatchTree();

        private bool _initialized;
        private bool _contentLoaded;
        private bool _componentsLocked;
        private readonly List<IGlyphComponent> _newComponents;
        protected internal readonly GlyphCompositeInjector Injector;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        protected abstract SchedulerHandlerBase SchedulerAssigner { get; }

        protected GlyphSchedulableBase(IDependencyInjector injector)
        {
            var compositeInjector = new GlyphCompositeInjector(this, injector.Resolve<IDependencyRegistry>(), injector.Resolve<IDependencyRegistry>(InjectionScope.Local));

            Injector = compositeInjector;

            foreach (Type type in GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IInterpreter<>)))
                Add(typeof(Receiver<>).MakeGenericType(type.GetGenericArguments()));

            Enabled = true;
            Visible = true;

            _newComponents = new List<IGlyphComponent>();
        }

        public T Add<T>()
            where T : IGlyphComponent
        {
            return Injector.Add<T>();
        }

        public IGlyphComponent Add(Type componentType)
        {
            return Injector.Add(componentType) as IGlyphComponent;
        }

        // TODO : Handle injection on changing children & parents
        public override sealed void Add(IGlyphComponent item)
        {
            if (_componentsLocked)
            {
                _newComponents.Add(item);
                return;
            }

            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();
            if (GetComponent(type) != null && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            base.Add(item);

            foreach (IGlyphComponent component in this)
                foreach (InjectablePropertyInfo injectable in InstanceManager.GetInfo(component.GetType()).InjectableProperties)
                    if (injectable.InjectableTargets.HasFlag(GlyphInjectableTargets.Fraternal)
                        && item.GetType().IsInstanceOfType(injectable.PropertyInfo.PropertyType))
                        injectable.PropertyInfo.SetValue(component, item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
            {
                SchedulerAssigner.AssignComponent(glyphObject);
                glyphObject.Injector.Parent = this;
            }
            else
                SchedulerAssigner.AssignComponent(item);

            if (_initialized)
                item.Initialize();

            if (_contentLoaded)
            {
                var loadContent = item as ILoadContent;
                if (loadContent != null)
                    loadContent.LoadContent(Injector.Resolve<ContentLibrary>());
            }
        }

        public override sealed void Remove(IGlyphComponent item)
        {
            if (!Contains(item))
                throw new ArgumentException("Component provided is not contained by this entity !");

            base.Remove(item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                SchedulerAssigner.RemoveComponent(glyphObject);
            else
                SchedulerAssigner.RemoveComponent(item);
        }

        public override sealed void Clear()
        {
            base.Clear();

            SchedulerAssigner.ClearComponents();
        }

        protected void LockComponents()
        {
            _componentsLocked = true;
        }

        protected void UnlockComponents()
        {
            _componentsLocked = false;

            foreach (IGlyphComponent newComponent in _newComponents)
                Add(newComponent);

            _newComponents.Clear();
        }

        public override sealed void Initialize()
        {
            LockComponents();

            foreach (InitializeDelegate initialize in SchedulerAssigner.Initialize.TopologicalOrder)
                initialize();

            _initialized = true;

            UnlockComponents();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            LockComponents();

            foreach (LoadContentDelegate loadContent in SchedulerAssigner.LoadContent.TopologicalOrder)
                loadContent(contentLibrary);

            _contentLoaded = true;

            UnlockComponents();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            LockComponents();

            foreach (UpdateDelegate update in SchedulerAssigner.Update.TopologicalOrder)
            {
                if (!Enabled)
                    return;

                using (UpdateWatchTree.Start($"{update.Target.GetType().GetDisplayName()} -- {update.Method.Name}"))
                    update(elapsedTime);
            }

            UnlockComponents();
        }

        public abstract void Draw(IDrawer drawer);

        public void SendMessage<TMessage>(TMessage message)
            where TMessage : Message
        {
            var router = Injector.Resolve<IRouter<TMessage>>();
            router.Send(message);
        }

        public class SchedulerHandlerBase : GlyphSchedulerHandler
        {
            public IGlyphScheduler<IGlyphComponent, InitializeDelegate> Initialize { get; private set; }
            public IGlyphScheduler<ILoadContent, LoadContentDelegate> LoadContent { get; private set; }
            public IGlyphScheduler<IUpdate, UpdateDelegate> Update { get; private set; }

            public SchedulerHandlerBase(IDependencyInjector injector)
                : base(injector)
            {
                Initialize = Add<IGlyphComponent, InitializeDelegate>();
                LoadContent = Add<ILoadContent, LoadContentDelegate>();
                Update = Add<IUpdate, UpdateDelegate>();
            }
        }
    }
}