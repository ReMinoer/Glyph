using System;
using System.Collections.Generic;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes
{
    public interface IGridMeshing<in TGrid, in T, TInfo>
        where TGrid : IGrid<T>
    {
        event EventHandler Changed;

        TInfo EmptyInfo { get; }
        TInfo GetCellInfo(TGrid grid, int i, int j, int gridColumns, int gridRows);

        bool CanContainsRectangles { get; }
        bool IsPartOfRectangle(TInfo cellInfo);
        void AddRectangle(ICollection<Vector2> vertices, TGrid grid, Rectangle rectangle);

        bool IsCell(TInfo cellInfo);
        void AddCell(ICollection<Vector2> vertices, TGrid grid, int i, int j, TInfo info);
    }
}