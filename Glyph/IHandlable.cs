namespace Glyph
{
    public interface IHandlable
    {
        bool IsHandled { get; }
        void Handle();
    }
}