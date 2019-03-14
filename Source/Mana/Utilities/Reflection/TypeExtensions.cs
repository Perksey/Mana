using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mana.Utilities.Reflection
{
    /// <summary>
    /// A static class of Type extension methods for making reflection operations
    /// more readable and concise.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets a value that indicates whether the type is decorated with a given
        /// attribute.
        /// </summary>
        /// <typeparam name="T">The type of the custom attribute.</typeparam>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="inherit">Whether to check if the type inherits the attribute from
        /// a parent class.</param>
        /// <returns>A value that indicates whether the type is decorated with a given attribute.</returns>
        public static bool HasAttribute<T>(this Type type, bool inherit = true)
            where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Any();
        }

        /// <summary>
        /// Gets an enumerable collection of attribute objects of a given custom
        /// attribute type that are decorating the type.
        /// </summary>
        /// <typeparam name="T">The type of the custom attribute.</typeparam>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="inherit">Whether to check if the type inherits the attribute from
        /// a parent class.</param>
        /// <returns>An enumerable collection of attribute objects of a given custom attribute type.</returns>
        public static IEnumerable<T> GetAttributes<T>(this Type type, bool inherit = true)
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Gets an attribute object of the given attribute type decorating the type.
        /// If not present, 'null' will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the custom attribute.</typeparam>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="inherit">Whether to check if the type inherits the attribute from
        /// a parent class.</param>
        /// <returns>An attribute object of the given type, or null if none is present.</returns>
        public static T GetAttribute<T>(this Type type, bool inherit = true)
            where T : Attribute
        {
            return GetAttributes<T>(type, inherit).FirstOrDefault();
        }

        /// <summary>
        /// Gets a value that indicates whether the type has a given parent type
        /// anywhere above it in its type hierarchy.
        /// </summary>
        /// <typeparam name="T">The parent type.</typeparam>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <returns>A value that indicates whether type has a given parent type.</returns>
        public static bool HasParent<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type) && typeof(T) != type;
        }

        /// <summary>
        /// Gets a value that indicates whether the type implements the given interface.
        /// </summary>
        /// <typeparam name="T">The interface type.</typeparam>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <returns>A value that indicates whether type implements a given interface.</returns>
        public static bool Implements<T>(this Type type)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Generic type parameter T must be an interface.");
            }

            return HasParent<T>(type);
        }

        /// <summary>
        /// Gets an enumerable collection of all fields in the type that satisfy a given predicate.
        /// </summary>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="predicate">The predicate that the retrieved FieldInfo objects must match.</param>
        /// <returns>An enumerable collection of all fields in the type that satisfy a given predicate.</returns>
        public static IEnumerable<FieldInfo> GetFields(this Type type,
                                                       Func<FieldInfo, bool> predicate)
        {
            return type.GetFields().Where(predicate);
        }

        /// <summary>
        /// Gets an enumerable collection of all fields in the type that satisfy a given predicate.
        /// </summary>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="bindingAttr">The binding flags describing which fields to get.</param>
        /// <param name="predicate">The predicate that the retrieved FieldInfo objects must match.</param>
        /// <returns>An enumerable collection of all fields in the type that satisfy a given predicate.</returns>
        public static IEnumerable<FieldInfo> GetFields(this Type type,
                                                       BindingFlags bindingAttr,
                                                       Func<FieldInfo, bool> predicate)
        {
            return type.GetFields(bindingAttr).Where(predicate);
        }

        /// <summary>
        /// Gets the first interface on the given type that satisfies the given predicate, or null if none could be found.
        /// </summary>
        /// <param name="type">The Type object to perform this operation on.</param>
        /// <param name="predicate">The predicate that the retrieved Type must match.</param>
        /// <returns>The first interface on the given type that satisfies the given predicate, or null if none could be found.</returns>
        public static Type GetInterface(this Type type, Func<Type, bool> predicate)
        {
            return type.GetInterfaces().FirstOrDefault(predicate);
        }
    }
}
