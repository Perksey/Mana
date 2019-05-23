using System.Numerics;

namespace Mana.Utilities.Extensions
{
    public static class Matrix4x4Extensions
    {
        public static Matrix4x4 GetInverse(this Matrix4x4 matrix)
        {
            bool success = Matrix4x4.Invert(matrix, out Matrix4x4 inverse);
            Assert.That(success);
            return inverse;
        }
    }
}