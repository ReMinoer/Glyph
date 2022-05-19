using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Tools.Snapping
{
    public class GridComponentsSnapping : IDisposable
    {
        private readonly MessagingSpace<IGridComponent> _gridComponentSpace;

        public GridComponentsSnapping(IGlyphComponent root)
        {
            _gridComponentSpace = new MessagingSpace<IGridComponent>(root);
        }

        public void Dispose()
        {
            _gridComponentSpace.Dispose();
        }

        public Vector2 Snap(IGlyphComponent component, Vector2 oldPosition, Vector2 newPosition,
            out float? horizontalSnap, out float? verticalSnap)
        {
            horizontalSnap = null;
            verticalSnap = null;

            float nearestHorizontalDiff = float.MaxValue;
            float nearestVerticalDiff = float.MaxValue;

            float horizontalSnappingPosition = newPosition.X;
            float verticalSnappingPosition = newPosition.Y;
            
            IGlyphComponent[] parents = component.AndAllParents().ToArray();

            foreach (IGridComponent gridComponent in _gridComponentSpace
                         .Where(x => !parents.Contains(x) && !x.AndAllParents<IGlyphComponent>().Contains(component)))
            {
                IGridSnapped gridSnapped = gridComponent as IGridSnapped;

                IEnumerable<float> horizontalSnapperPositions;
                IEnumerable<float> verticalSnapperPositions;
                if (gridSnapped != null)
                {
                    horizontalSnapperPositions = gridSnapped.GetHorizontalSnapperPositions(oldPosition, component);
                    verticalSnapperPositions = gridSnapped.GetVerticalSnapperPositions(oldPosition, component);
                }
                else
                {
                    horizontalSnapperPositions = new[] { oldPosition.X };
                    verticalSnapperPositions = new[] { oldPosition.Y };
                }
                
                foreach (float horizontalSnapperPosition in horizontalSnapperPositions)
                    CheckHorizontalSide(horizontalSnapperPosition, ref horizontalSnap);
                foreach (float verticalSnapperPosition in verticalSnapperPositions)
                    CheckVerticalSide(verticalSnapperPosition, ref verticalSnap);

                void CheckHorizontalSide(float sidePosition, ref float? sideSnap)
                {
                    float sideGap = sidePosition - oldPosition.X;
                    float newSidePosition = newPosition.X + sideGap;

                    IEnumerable<float> snappedPositions = gridSnapped != null
                        ? gridSnapped.GetHorizontalSnappedPositions(newPosition)
                        : new[] { gridComponent.Grid.ToWorldPoint(gridComponent.Grid.ToGridPoint(newPosition.SetX(newSidePosition))).X };

                    foreach (float snappedPosition in snappedPositions)
                        CheckBoxSide(snappedPosition, ref sideSnap);

                    void CheckBoxSide(float targetSide, ref float? snap)
                    {
                        float diff = System.Math.Abs(targetSide - newSidePosition);
                        if (diff < nearestHorizontalDiff)
                        {
                            nearestHorizontalDiff = diff;
                            snap = targetSide;
                            horizontalSnappingPosition = targetSide - sideGap;
                        }
                    }
                }

                void CheckVerticalSide(float sidePosition, ref float? sideSnap)
                {
                    float sideGap = sidePosition - oldPosition.Y;
                    float newSidePosition = newPosition.Y + sideGap;

                    IEnumerable<float> snappedPositions = gridSnapped != null
                        ? gridSnapped.GetVerticalSnappedPositions(newPosition)
                        : new[] { gridComponent.Grid.ToWorldPoint(gridComponent.Grid.ToGridPoint(newPosition.SetY(newSidePosition))).Y };

                    foreach (float snappedPosition in snappedPositions)
                        CheckBoxSide(snappedPosition, ref sideSnap);

                    void CheckBoxSide(float targetSide, ref float? snap)
                    {
                        float diff = System.Math.Abs(targetSide - newSidePosition);
                        if (diff < nearestVerticalDiff)
                        {
                            nearestVerticalDiff = diff;
                            snap = targetSide;
                            verticalSnappingPosition = targetSide - sideGap;
                        }
                    }
                }
            }

            return new Vector2(horizontalSnappingPosition, verticalSnappingPosition);
        }
    }

    public interface IGridSnapped
    {
        IEnumerable<float> GetHorizontalSnappedPositions(Vector2 position);
        IEnumerable<float> GetVerticalSnappedPositions(Vector2 position);
        IEnumerable<float> GetHorizontalSnapperPositions(Vector2 position, IGlyphComponent component);
        IEnumerable<float> GetVerticalSnapperPositions(Vector2 position, IGlyphComponent component);
    }
}