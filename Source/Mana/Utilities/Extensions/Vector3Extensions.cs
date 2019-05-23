using System.Numerics;
using System.Runtime.CompilerServices;

namespace Mana.Utilities.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Returns the normalized vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalized(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }

        /// <summary>
        /// Converts the <see cref="Vector3"/> object to a <see cref="Vector2"/> object,
        /// truncating the Z component.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return Unsafe.As<Vector3, Vector2>(ref vector);
        }
    }
}