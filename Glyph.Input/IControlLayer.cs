using System.Collections.Generic;
using Fingear;

namespace Glyph.Input
{
    public interface IControlLayer : IEnumerable<IControl>
    {
        bool Enabled { get; set; }
        string DisplayName { get; }
    }
}