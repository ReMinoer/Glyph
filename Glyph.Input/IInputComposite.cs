using System.Collections.Generic;

namespace Glyph.Input
{
    public interface IInputComposite : IInputHandler, ICollection<IInputHandler>
    {
    }
}