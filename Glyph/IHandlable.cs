using System.Security.Cryptography.X509Certificates;

namespace Glyph
{
    public interface IHandlable
    {
        bool IsHandled { get; }
        void Handle();
    }
}