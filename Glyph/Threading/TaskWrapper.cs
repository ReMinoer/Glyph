using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Glyph.Threading
{

    public class TaskWrapper : ITask
    {
        private readonly Task _task;

        public TaskWrapper(Task task)
        {
            _task = task;
        }

        public void Wait() => _task.Wait();
        public IAwaiter GetAwaiter() => new Awaiter(_task);
        public IAwaitable<IAwaiter> SkipContextCapture() => new SkipContextCaptureAwaitable(_task);

        private class Awaiter : IAwaiter
        {
            private TaskAwaiter _taskAwaiter;

            public Awaiter(Task task)
            {
                _taskAwaiter = task.GetAwaiter();
            }

            public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);
            public bool IsCompleted => _taskAwaiter.IsCompleted;
            public void GetResult() => _taskAwaiter.GetResult();
        }

        private class SkipContextCaptureAwaitable : IAwaitable<IAwaiter>
        {
            private readonly ConfiguredTaskAwaitable _configuredTaskAwaitable;

            public SkipContextCaptureAwaitable(Task task)
            {
                _configuredTaskAwaitable = task.ConfigureAwait(true);
            }

            public IAwaiter GetAwaiter() => new SkipContextCaptureAwaiter(_configuredTaskAwaitable);
        }

        private class SkipContextCaptureAwaiter : IAwaiter
        {
            private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _taskAwaiter;

            public SkipContextCaptureAwaiter(ConfiguredTaskAwaitable configuredTaskAwaitable)
            {
                _taskAwaiter = configuredTaskAwaitable.GetAwaiter();
            }

            public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);
            public bool IsCompleted => _taskAwaiter.IsCompleted;
            public void GetResult() => _taskAwaiter.GetResult();
        }
    }

    public class TaskWrapper<T> : ITask<T>
    {
        private Task<T> _task;

        public TaskWrapper(Task<T> task)
        {
            _task = task;
        }

        public T Result => _task.Result;
        public IAwaiter<T> GetAwaiter() => new Awaiter(_task);
        public IAwaitable<IAwaiter<T>> SkipContextCapture() => new SkipContextCaptureAwaitable(_task);

        private class Awaiter : IAwaiter<T>
        {
            private TaskAwaiter<T> _taskAwaiter;

            public Awaiter(Task<T> task)
            {
                _taskAwaiter = task.GetAwaiter();
            }

            public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);
            public bool IsCompleted => _taskAwaiter.IsCompleted;
            public T GetResult() => _taskAwaiter.GetResult();
        }

        private class SkipContextCaptureAwaitable : IAwaitable<IAwaiter<T>>
        {
            private readonly ConfiguredTaskAwaitable<T> _configuredTaskAwaitable;

            public SkipContextCaptureAwaitable(Task<T> task)
            {
                _configuredTaskAwaitable = task.ConfigureAwait(true);
            }

            public IAwaiter<T> GetAwaiter() => new SkipContextCaptureAwaiter(_configuredTaskAwaitable);
        }

        private class SkipContextCaptureAwaiter : IAwaiter<T>
        {
            private ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter _taskAwaiter;

            public SkipContextCaptureAwaiter(ConfiguredTaskAwaitable<T> configuredTaskAwaitable)
            {
                _taskAwaiter = configuredTaskAwaitable.GetAwaiter();
            }

            public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);
            public bool IsCompleted => _taskAwaiter.IsCompleted;
            public T GetResult() => _taskAwaiter.GetResult();
        }
    }
}