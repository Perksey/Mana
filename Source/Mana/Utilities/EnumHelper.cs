using System;
using System.Linq;

namespace Mana.Utilities
{
    /// <summary>
    /// A static class of helper functions for dealing with enums.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets an array containing all values of an enum type.
        /// </summary>
        /// <typeparam name="TEnum">The type whose values to get.</typeparam>
        /// <returns>An array containing all values of an enum type.</returns>
        public static TEnum[] GetValues<TEnum>()
            where TEnum : struct, Enum
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type.");
            }

            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .ToArray();
        }
    }
}
