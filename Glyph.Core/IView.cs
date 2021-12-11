using System;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Scheduling;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public interface IView : IDraw, IShape, IBoxedComponent, ITransformer
    {
        Vector2 Size { get; }
        ICamera Camera { get; set; }
        Quad DisplayedRectangle { get; }
        Matrix RenderMatrix { get; }
        RenderScheduler RenderScheduler { get; }
        event EventHandler<Vector2> SizeChanged;
        event EventHandler<ICamera> CameraChanged;
    }
}