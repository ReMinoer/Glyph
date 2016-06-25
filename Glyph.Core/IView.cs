using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Core
{
    public interface IView : ILoadContent, IUpdate, IDraw, IArea
    {
        ICamera Camera { get; }
        CenteredRectangle DisplayedRectangle { get; }
        Texture2D Output { get; }
        Matrix Matrix { get; }
        bool IsVisibleOnView(Vector2 position);
        Vector2 GetPositionOnView(Vector2 position);
        void PrepareDraw(IDrawer drawer);
        void ApplyEffects(IDrawer drawer);
    }
}