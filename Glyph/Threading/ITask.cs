namespace Glyph.Threading
{
    public interface ITask : IAwaitable<IAwaiter>
    {
        void Wait();
        IAwaitable<IAwaiter> SkipContextCapture();
    }

    public interface ITask<out T> : IAwaitable<IAwaiter<T>>
    {
        T Result { get; }
        IAwaitable<IAwaiter<T>> SkipContextCapture();
    }
}