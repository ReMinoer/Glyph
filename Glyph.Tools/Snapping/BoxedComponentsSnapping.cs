using System;
using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Tools.Snapping
{
    public class BoxedComponentsSnapping : IDisposable
    {
        private readonly MessagingSpace<IBoxedComponent> _boxedComponentSpace;

        public BoxedComponentsSnapping(IGlyphComponent root)
        {
            _boxedComponentSpace = new MessagingSpace<IBoxedComponent>(root);
        }

        public void Dispose()
        {
            _boxedComponentSpace.Dispose();
        }

        public Vector2 Snap(IGlyphComponent component, Vector2 oldPosition, Vector2 newPosition,
            out float? horizontalSnap, out float? verticalSnap)
        {
            horizontalSnap = null;
            verticalSnap = null;

            float nearestHorizontalDiff = float.MaxValue;
            float nearestVerticalDiff = float.MaxValue;

            float horizontalSnappedPosition = newPosition.X;
            float verticalSnappedPosition = newPosition.Y;

            IGlyphComponent[] parents = component.AndAllParents().ToArray();

            IBoxedComponent boxedComponent = component as IBoxedComponent;
            Vector2 snapRange = boxedComponent != null
                ? boxedComponent.Area.BoundingBox.Size / 4
                : new Vector2(25, 25);

            foreach (TopLeftRectangle box in _boxedComponentSpace
                         .Where(x => !parents.Contains(x) && !x.AndAllParents<IGlyphComponent>().Contains(component))
                         .Select(x => x.Area.BoundingBox))
            {
                if (boxedComponent is null)
                {
                    CheckHorizontalSide(oldPosition.X, ref horizontalSnap);
                    CheckVerticalSide(oldPosition.Y, ref verticalSnap);
                }
                else
                {
                    CheckHorizontalSide(boxedComponent.Area.BoundingBox.Center.X, ref horizontalSnap);
                    CheckHorizontalSide(boxedComponent.Area.BoundingBox.Left, ref horizontalSnap);
                    CheckHorizontalSide(boxedComponent.Area.BoundingBox.Right, ref horizontalSnap);

                    CheckVerticalSide(boxedComponent.Area.BoundingBox.Center.Y, ref verticalSnap);
                    CheckVerticalSide(boxedComponent.Area.BoundingBox.Top, ref verticalSnap);
                    CheckVerticalSide(boxedComponent.Area.BoundingBox.Bottom, ref verticalSnap);
                }

                void CheckHorizontalSide(float sidePosition, ref float? sideSnap)
                {
                    float sideGap = sidePosition - oldPosition.X;
                    float newSidePosition = newPosition.X + sideGap;

                    CheckBoxSide(box.Left, ref sideSnap);
                    CheckBoxSide(box.Right, ref sideSnap);

                    void CheckBoxSide(float targetSide, ref float? snap)
                    {
                        float diff = System.Math.Abs(targetSide - newSidePosition);
                        if (diff < nearestHorizontalDiff && diff <= snapRange.X)
                        {
                            nearestHorizontalDiff = diff;
                            snap = targetSide;
                            horizontalSnappedPosition = targetSide - sideGap;
                        }
                    }
                }

                void CheckVerticalSide(float sidePosition, ref float? sideSnap)
                {
                    float sideGap = sidePosition - oldPosition.Y;
                    float newSidePosition = newPosition.Y + sideGap;

                    CheckBoxSide(box.Top, ref sideSnap);
                    CheckBoxSide(box.Bottom, ref sideSnap);

                    void CheckBoxSide(float targetSide, ref float? snap)
                    {
                        float diff = System.Math.Abs(targetSide - newSidePosition);
                        if (diff < nearestVerticalDiff && diff <= snapRange.Y)
                        {
                            nearestVerticalDiff = diff;
                            snap = targetSide;
                            verticalSnappedPosition = targetSide - sideGap;
                        }
                    }
                }
            }

            return new Vector2(horizontalSnappedPosition, verticalSnappedPosition);
        }
    }
}