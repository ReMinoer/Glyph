using Glyph.Math;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Glyph.Tests.Math
{
    [TestFixture]
    static public class TopLeftRectangleTest
    {
        [Test]
        static public void Ctor()
        {
            var result = new Transformation();
            Assert.AreEqual(Vector2.Zero, result.Translation);
            Assert.AreEqual(0, result.Rotation);
            Assert.AreEqual(1, result.Scale);
            Assert.AreEqual(Matrix3X3.Identity, result.Matrix);

            result = Transformation.Identity;
            Assert.AreEqual(Vector2.Zero, result.Translation);
            Assert.AreEqual(0, result.Rotation);
            Assert.AreEqual(1, result.Scale);
            Assert.AreEqual(Matrix3X3.Identity, result.Matrix);

            var translation = new Vector2(5, 2);
            const float rotation = MathHelper.PiOver4;
            const float scale = 3;

            result = new Transformation(translation, rotation, scale);
            Assert.AreEqual(translation, result.Translation);
            Assert.AreEqual(rotation, result.Rotation);
            Assert.AreEqual(scale, result.Scale);
            Assert.AreEqual(new Matrix3X3Trs(translation, rotation, scale), result.Matrix);

            result = new Transformation(result);
            Assert.AreEqual(translation, result.Translation);
            Assert.AreEqual(rotation, result.Rotation);
            Assert.AreEqual(scale, result.Scale);
            Assert.AreEqual(new Matrix3X3Trs(translation, rotation, scale), result.Matrix);

            result = new Transformation { Translation = translation, Rotation = rotation, Scale = scale };
            Assert.AreEqual(translation, result.Translation);
            Assert.AreEqual(rotation, result.Rotation);
            Assert.AreEqual(scale, result.Scale);
            Assert.AreEqual(new Matrix3X3Trs(translation, rotation, scale), result.Matrix);
        }

        [Test]
        static public void RefreshMatrix()
        {
            Transformation result = Transformation.Identity;

            var translation = new Vector2(5, 2);
            const float rotation = MathHelper.PiOver4;
            const float scale = 3;

            result.RefreshMatrix(translation, rotation, scale);
            Assert.AreEqual(translation, result.Translation);
            Assert.AreEqual(rotation, result.Rotation);
            Assert.AreEqual(scale, result.Scale);
            Assert.AreEqual(new Matrix3X3Trs(translation, rotation, scale), result.Matrix);
        }
    }
}