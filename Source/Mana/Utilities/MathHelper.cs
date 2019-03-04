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
        public static float DegreesToRadians(float degrees)
        {
            return degrees * PiOver180;
        }

        /// <summary>
        /// Returns the given radians value in degrees.
        /// </summary>
        /// <param name="radians">The radians value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansToDegrees(float radians)
        {
            return radians / PiOver180;
        }

        /// <summary>
        /// Returns the given value, clamped to the given minimum and maximum bounds.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum (lower bound).</param>
        /// <param name="max">The maximum (upper bound).</param>
        /// <returns>The given value, clamped to the given minimum and maximum bounds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Returns the given value, clamped to the given minimum and maximum bounds.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum (lower bound).</param>
        /// <param name="max">The maximum (upper bound).</param>
        /// <returns>The given value, clamped to the given minimum and maximum bounds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Returns the given value, clamped to be between the range [0-1].
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The given value, clamped to be between the range [0-1].</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            return value < 0f ? 0f : value > 1f ? 1f : value;
        }

        /// <summary>
        /// Returns the result of a linear interpolation between two given values by a given amount.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The amount to use to interpolate between the two values.</param>
        /// <returns>The result of the linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float amount)
        {
            return a + ((b - a) * amount);
        }

        /// <summary>
        /// Returns the result of a linear interpolation between two given values by a given amount.
        /// This method will clamp the amount to the range [0, 1] before performing the operation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The value to interpolate between the two values.</param>
        /// <returns>The result of the linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpClamped(float a, float b, float amount)
        {
            return a + ((b - a) * Clamp01(amount));
        }

        /// <summary>
        /// Returns an inverse linear interpolation of an amount between two given values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The value to use to interpolate between the two values.</param>
        /// <returns>The result of the inverse linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float a, float b, float amount)
        {
            return (amount - a) / (b - a);
        }

        /// <summary>
        /// Returns an inverse linear interpolation of an amount between two given values.
        /// This method will clamp the result between the range [0, 1] after performing the operation.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="amount">The value to use to interpolate between the two values.</param>
        /// <returns>The result of the inverse linear interpolation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerpClamped(float a, float b, float amount)
        {
            return Clamp01((amount - a) / (b - a));
        }

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
        public static float RangeMap(float a1, float a2, float b1, float b2, float value)
        {
            return Lerp(b1, b2, (value - a1) / (a2 - a1));
        }

        /// <summary>
        /// Returns a value indicating whether the given integer is a power of two.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A value indicating whether the given integer is a power of two.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

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

        public static bool WithinEpsilon(float a, float b)
        {
            return Math.Abs(a - b) < float.Epsilon;
        }
    }
}