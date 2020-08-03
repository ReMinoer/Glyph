namespace Glyph.Threading
{
    public interface IAwaitable<out TAwaiter>
    {
        TAwaiter GetAwaiter();
    }
}