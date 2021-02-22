using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Scheduling;
using Glyph.Scheduling.Base;

namespace Glyph.Core.Schedulers.Base
{
    public abstract class ComponentSchedulerBase<T, TDelegate>
        : IGlyphScheduler<T, TDelegate, ComponentSchedulerBase<T, TDelegate>.TaskController, ComponentSchedulerBase<T, TDelegate>.Controller>
    {
        private readonly GlyphSchedulerBase<T, TDelegate> _glyphScheduler;
        private IGlyphScheduler<T, TDelegate> GlyphScheduler => _glyphScheduler;

        private readonly PriorityGroupDictionary _priorityGroups = new PriorityGroupDictionary();

        public ComponentSchedulerBase(GlyphSchedulerBase<T, TDelegate> glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;

            _priorityGroups.Add(Priority.High, new PriorityGroup());
            _priorityGroups.Add(Priority.Normal, new PriorityGroup());
            _priorityGroups.Add(Priority.Low, new PriorityGroup());
            
            glyphScheduler.Plan(_priorityGroups[Priority.High]).Before(_priorityGroups[Priority.Normal]);
            glyphScheduler.Plan(_priorityGroups[Priority.Normal]).Before(_priorityGroups[Priority.Low]);
        }

        public TaskController Plan(T task)
        {
            GlyphScheduler.Plan(task);
            InitPriority(task);

            return new TaskController(this, _glyphScheduler, new[] { task });
        }

        public TaskController Plan(IEnumerable<T> tasks)
        {
            GlyphScheduler.Plan(tasks);
            foreach (T task in tasks)
                InitPriority(task);

            return new TaskController(this, _glyphScheduler, tasks);
        }

        public TaskController Plan(TDelegate taskDelegate)
        {
            GlyphScheduler.Plan(taskDelegate);

            T task = _glyphScheduler.GetDelegateTask(taskDelegate);
            InitPriority(task);

            return new TaskController(this, _glyphScheduler, new[] { task });
        }

        void IGlyphScheduler<T>.Plan(T task) => Plan(task);
        void IGlyphScheduler<T>.Plan(IEnumerable<T> tasks) => Plan(tasks);
        void IGlyphScheduler<T, TDelegate>.Plan(TDelegate taskDelegate) => Plan(taskDelegate);

        public Controller Plan<TTasks>() => Plan(typeof(TTasks));
        public Controller Plan(Type taskType) => new Controller(this, _glyphScheduler, _glyphScheduler.GetOrAddTypedGroup(taskType));

        public void Unplan(T task)
        {
            CleanPriority(task);
            GlyphScheduler.Unplan(task);
        }

        public void Unplan(TDelegate taskDelegate) => Unplan(_glyphScheduler.GetDelegateTask(taskDelegate));

        private void InitPriority(T task)
        {
            if (!_priorityGroups[Priority.Normal].Contains(task))
                _priorityGroups[Priority.Normal].Add(task);
        }

        private void CleanPriority(T task)
        {
            foreach (PriorityGroup priorityGroup in _priorityGroups.Values)
                priorityGroup.Remove(task);
        }

        public class Controller : ControllerBase<Controller>
        {
            public Controller(ComponentSchedulerBase<T, TDelegate> componentScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> tasks)
                : base(componentScheduler, glyphScheduler, tasks)
            {
            }

            protected override Controller ReturnedController => this;
        }

        public class TaskController : ControllerBase<TaskController>
        {
            private Priority _priority;

            public TaskController(ComponentSchedulerBase<T, TDelegate> componentScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> controlledTasks)
                : base(componentScheduler, glyphScheduler, controlledTasks)
            {
                _priority = Priority.Normal;
            }

            protected override TaskController ReturnedController => this;

            public TaskController AtStart()
            {
                SwitchPriorityGroup(Priority.High);
                return ReturnedController;
            }

            public TaskController AtEnd()
            {
                SwitchPriorityGroup(Priority.Low);
                return ReturnedController;
            }

            private void SwitchPriorityGroup(Priority priority)
            {
                foreach (T controlledTask in ControlledTasks)
                    ComponentScheduler._priorityGroups[_priority].Remove(controlledTask);

                _priority = priority;

                foreach (T controlledTask in ControlledTasks)
                    ComponentScheduler._priorityGroups[_priority].Add(controlledTask);
            }
        }

        public abstract class ControllerBase<TController> : GlyphSchedulerBase<T, TDelegate>.ControllerBase<TController>
        {
            protected readonly ComponentSchedulerBase<T, TDelegate> ComponentScheduler;

            public ControllerBase(ComponentSchedulerBase<T, TDelegate> componentScheduler, GlyphSchedulerBase<T, TDelegate> glyphScheduler, IEnumerable<T> tasks)
                : base(glyphScheduler, tasks)
            {
                ComponentScheduler = componentScheduler;
            }
        }

        private enum Priority
        {
            High,
            Normal,
            Low
        }

        private class PriorityGroupDictionary : Dictionary<Priority, PriorityGroup>
        {
        }

        private class PriorityGroup : ObservableCollection<T>
        {
        }
    }
}