using System.Numerics;
using System.Runtime.CompilerServices;

namespace Mana.Utilities
{
    public static class Vector3Helper
    {
        /// <summary>
        /// Gets a <see cref="Vector3"/> representing an up-facing unit vector. (0f, 1f, 0f)
        /// </summary>
        public static Vector3 Up => new Vector3(0f, 1f, 0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> representing a down-facing unit vector. (0f, -1f, 0f)
        /// </summary>
        public static Vector3 Down => new Vector3(0f, -1f, 0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> representing a left-facing unit vector. (-1f, 0f, 0f)
        /// </summary>
        public static Vector3 Left => new Vector3(-1f, 0f, 0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> representing a right-facing unit vector. (1f, 0f, 0f)
        /// </summary>
        public static Vector3 Right => new Vector3(1f, 0f, 0f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> representing a forward-facing unit vector. (0f, 0f, -1f)
        /// </summary>
        public static Vector3 Forward => new Vector3(0f, 0f, -1f);

        /// <summary>
        /// Gets a <see cref="Vector3"/> representing a backward-facing unit vector. (0f, 0f, 1f)
        /// </summary>
        public static Vector3 Backward => new Vector3(0f, 0f, 1f);
        
        /// <summary>
        /// Gets the midpoint between two <see cref="Vector3"/> objects.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Midpoint(Vector3 a, Vector3 b)
        {
            return (a + b) / 2.0f;
        }

        /// <summary>
        /// Gets the midpoint between three <see cref="Vector3"/> objects.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Midpoint(Vector3 a, Vector3 b, Vector3 c)
        {
            return (a + b + c) / 3.0f;
        }

        /// <summary>
        /// Gets the midpoint between four <see cref="Vector3"/> objects.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Midpoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return (a + b + c + d) / 4.0f;
        }
    }
}