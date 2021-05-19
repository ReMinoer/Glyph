using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Glyph.Tests.Math
{
    [TestFixture]
    static public class MathUtilsTest
    {
        [Test]
        static public void GetBoundingBox()
        {
            var pointA = new Vector2(-2, 1);
            var pointB = new Vector2(2, 3);
            var pointC = new Vector2(-3, -4);

            TopLeftRectangle result = MathUtils.GetBoundingBox(pointA, pointB, pointC);
            Assert.AreEqual(-3, result.Left);
            Assert.AreEqual(2, result.Right);
            Assert.AreEqual(-4, result.Top);
            Assert.AreEqual(3, result.Bottom);

            result = MathUtils.GetBoundingBox((IEnumerable<Vector2>)null);
            Assert.AreEqual(0, result.Left);
            Assert.AreEqual(0, result.Right);
            Assert.AreEqual(0, result.Top);
            Assert.AreEqual(0, result.Bottom);

            result = MathUtils.GetBoundingBox(new Vector2[0]);
            Assert.AreEqual(0, result.Left);
            Assert.AreEqual(0, result.Right);
            Assert.AreEqual(0, result.Top);
            Assert.AreEqual(0, result.Bottom);

            var rectangleA = new TopLeftRectangle(-3, -2, 1, 1);
            var rectangleB = new TopLeftRectangle(2, 1, 3, 3);
            var rectangleC = new TopLeftRectangle(-4, -5, 2, 10);

            result = MathUtils.GetBoundingBox(rectangleA, rectangleB, rectangleC);
            Assert.AreEqual(-4, result.Left);
            Assert.AreEqual(5, result.Right);
            Assert.AreEqual(-5, result.Top);
            Assert.AreEqual(5, result.Bottom);

            result = MathUtils.GetBoundingBox((IEnumerable<TopLeftRectangle>)null);
            Assert.AreEqual(0, result.Left);
            Assert.AreEqual(0, result.Right);
            Assert.AreEqual(0, result.Top);
            Assert.AreEqual(0, result.Bottom);

            result = MathUtils.GetBoundingBox(new TopLeftRectangle[0]);
            Assert.AreEqual(0, result.Left);
            Assert.AreEqual(0, result.Right);
            Assert.AreEqual(0, result.Top);
            Assert.AreEqual(0, result.Bottom);

            var circleA = new Circle(new Vector2(-2, -2), 3);
            var circleB = new Circle(new Vector2(2, 2), 5);
            var circleC = new Circle(new Vector2(5, 8), 2);

            result = MathUtils.GetBoundingBox(circleA, circleB, circleC);
            Assert.AreEqual(-5, result.Left);
            Assert.AreEqual(7, result.Right);
            Assert.AreEqual(-5, result.Top);
            Assert.AreEqual(10, result.Bottom);
        }
    }
}