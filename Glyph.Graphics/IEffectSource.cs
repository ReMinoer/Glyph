using System;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IEffectSource : IGlyphComponent
    {
        Effect Effect { get; }
        event Action<IEffectSource> EffectLoaded;
    }
}