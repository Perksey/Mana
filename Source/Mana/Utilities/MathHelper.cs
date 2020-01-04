using System;
using System.Runtime.CompilerServices;

namespace Mana.Utilities
{
    /// <summary>
    /// A static helper class of useful mathematical functions and constants.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Represents the mathematical constant Pi.
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// Represents the mathematical constant Tau (2 * Pi).
        /// </summary>
        public const float Tau = (float)(Math.PI * 2f);

        /// <summary>
        /// Represents the mathematical constant Pi over 180.
        /// </summary>
        public const float PiOver180 = (float)(Math.PI / 180.0);

        /// <summary>
        /// Represents the mathematical constant E.
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Returns the given degrees value in radians.
        /// </summary>
        /// <param name="degrees">The degrees value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToRadians(float degrees) => degrees * PiOver180;

        /// <summary>
        /// Returns the given radians value in degrees.
        /// </summary>
        /// <param name="radians">The radians value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansToDegrees(float radians) => radians / PiOver180;

        /// <summary>
        /// Returns the given value, clamped to the given minimum and maximum bounds.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum (lower bound).</param>
        /// <param name="max">The maximum (upper bound).</param>
        /// <returns>The given value, clamped to the given minimum and maximum bounds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;

        /// <summary>
        /// Returns the given value, clamped to the given minimum and maximum bounds.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum (lower bound).</param>
        /// <param name="max">The maximum (upper bound).</param>
        /// <returns>The given value, clamped to the given minimum and maximum bounds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;

        /// <summary>
        /// Returns the given value, clamped to be between the range [0-1].
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The given value, clamped to be between the range [0-1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value) => Clamp(value, 0f, 1f);

        /// <summary>
        /// Returns the result of a linear interpolation between two given values by a given amount.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The amount to use to interpolate between the two values.</param>
        /// <returns>The result of the linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float amount) => a + ((b - a) * amount);

        /// <summary>
        /// Returns an inverse linear interpolation of an amount between two given values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The value to use to interpolate between the two values.</param>
        /// <returns>The result of the inverse linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float amount) => (amount - a) / (b - a);

        /// <summary>
        /// Returns an inverse linear interpolation of an amount between two given values.
        /// This method will clamp the result between the range [0, 1] after performing the operation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The value to use to interpolate between the two values.</param>
        /// <returns>The result of the inverse linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerpClamped(float a, float b, float amount) => Clamp01(InverseLerp(a, b, amount));

        /// <summary>
        /// Returns a value interpolated from it's position within one range, mapped to another range.
        /// </summary>
        /// <param name="a1">The first bound of the source range.</param>
        /// <param name="a2">The second bound of the source range.</param>
        /// <param name="b1">The first bound of the destination range.</param>
        /// <param name="b2">The second bound of the destination range.</param>
        /// <param name="value">The value to remap.</param>
        /// <returns>The result of the range mapping operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RangeMap(float a1, float a2, float b1, float b2, float value) => Lerp(b1, b2, InverseLerp(a1, a2, value));

        /// <summary>
        /// Returns a value indicating whether the given integer is a power of two.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A value indicating whether the given integer is a power of two.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value) => (value & (value - 1)) == 0;

        #region Single-Precision Trigonometry Functions

        /// <summary>
        /// Returns the sine of the given radians angle.
        /// </summary>
        /// <param name="radians">The radians angle.</param>
        /// <returns>The sine of the given radians angle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float radians)
        {
            return (float)Math.Sin(radians);
        }

        /// <summary>
        /// Returns the cosine of the given radians angle.
        /// </summary>
        /// <param name="radians">The radians angle.</param>
        /// <returns>The cosine of the given radians angle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float radians)
        {
            return (float)Math.Cos(radians);
        }

        /// <summary>
        /// Returns the tangent of the given radians angle.
        /// </summary>
        /// <param name="radians">The radians angle.</param>
        /// <returns>The tangent of the given radians angle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float radians)
        {
            return (float)Math.Tan(radians);
        }

        /// <summary>
        /// Returns the angle, in radians, whose sine is the given value.
        /// </summary>
        /// <param name="sine">The sine of the return value.</param>
        /// <returns>The angle, in radians, whose sine is the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Asin(float sine)
        {
            return (float)Math.Asin(sine);
        }

        /// <summary>
        /// Returns the angle, in radians, whose cosine is the given value.
        /// </summary>
        /// <param name="cosine">The cosine of the return value.</param>
        /// <returns>The angle, in radians, whose cosine is the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float cosine)
        {
            return (float)Math.Acos(cosine);
        }

        /// <summary>
        /// Returns the angle, in radians, whose tangent is the given value.
        /// </summary>
        /// <param name="tangent">The tangent of the return value.</param>
        /// <returns>The angle, in radians, whose tangent is the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float tangent)
        {
            return (float)Math.Atan(tangent);
        }

        /// <summary>
        /// Returns the angle, in radians, whose tangent is y/x.
        /// </summary>
        /// <param name="y">The numerator of the tangent of the return value.</param>
        /// <param name="x">The denominator of the tangent of the return value.</param>
        /// <returns>The angle, in radians, whose tangent is y/x.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        /// <summary>
        /// Returns the square root of the given value.
        /// </summary>
        /// <param name="value">The value to determine the square root of.</param>
        /// <returns>The square root of the given value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        #endregion


    }
}
