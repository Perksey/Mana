using System.Drawing;
using System.Numerics;

namespace Mana.Utilities.Extensions
{
    public static class Vector2Extensions
    {
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
    }
}