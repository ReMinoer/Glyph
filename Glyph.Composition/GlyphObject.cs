using System;
using System.Linq;
using System.Reflection;
using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Exceptions;
using Glyph.Composition.Scheduler;
using Glyph.Input;

namespace Glyph.Composition
{
    public class GlyphObject : GlyphComposite, IEnableable, ILoadContent, IUpdate, IHandleInput, IDraw
    {
        protected readonly IDependencyInjector Injector;
        protected readonly GlyphObjectScheduler Scheduler;
        public virtual bool Enabled { get; set; }
        public virtual bool Visible { get; set; }

        public GlyphObject(IDependencyInjector injector)
        {
            Injector = injector;
            Scheduler = injector.Resolve<GlyphObjectScheduler>();
        }

        public override void Initialize()
        {
            foreach (InitializeDelegate initialize in Scheduler.Initialize.TopologicalOrder)
                initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (LoadContentDelegate loadContent in Scheduler.LoadContent.TopologicalOrder)
                loadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (UpdateDelegate update in Scheduler.Update.TopologicalOrder)
                update(elapsedTime);
        }

        public void HandleInput(InputManager inputManager)
        {
            if (!Enabled)
                return;

            foreach (HandleInputDelegate handleInput in Scheduler.HandleInput.TopologicalOrder)
                handleInput(inputManager);
        }

        public void Draw()
        {
            if (!Visible)
                return;

            foreach (DrawDelegate draw in Scheduler.Draw.TopologicalOrder)
                draw();
        }

        public override void Dispose()
        {
            Clear();

            foreach (IGlyphComponent component in Components)
                component.Dispose();
        }

        public T Add<T>()
            where T : class, IGlyphComponent, new()
        {
            var component = Injector.Resolve<T>();
            Add(component);

            return component;
        }

        public IGlyphComponent Add(Type componentType)
        {
            var component = Injector.Resolve(componentType) as IGlyphComponent;
            Add(component);

            return component;
        }

        public override sealed void Add(IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();

            if (GetComponent(type) != null && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            base.Add(item);

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                Scheduler.Add(glyphObject);
            else
                Scheduler.Add(item);
        }

        public override sealed bool Remove(IGlyphComponent item)
        {
            if (!Contains(item))
                throw new ArgumentException("Component provided is not contained by this entity !");

            bool valid = base.Remove(item);

            if (!valid)
                return false;

            var glyphObject = item as GlyphObject;
            if (glyphObject != null)
                Scheduler.Remove(glyphObject);
            else
                Scheduler.Remove(item);

            return true;
        }

        public override sealed void Clear()
        {
            Scheduler.Clear();

            base.Clear();
        }
    }
}