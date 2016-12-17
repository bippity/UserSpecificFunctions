using System;
using System.Collections.Generic;
using System.Linq;

namespace UserSpecificFunctions.Extensions
{
    /// <summary>Provides a number of extension methods for the <see cref="List{T}"/> type.</summary>
    public static class ListExtensions
    {
        /// <summary>Checks the permission list to determine whether the given permission is negated.</summary>
        /// <param name="source">The permission list.</param>
        /// <param name="permission">The permission.</param>
        /// <returns>True or false.</returns>
        [Obsolete("No longer required as this is handled by PermissionList.")]
        public static bool Negated(this List<string> source, string permission)
        {
            return source.Any(p => p.StartsWith("!") && p.Substring(1) == permission);
        }

        /// <summary>Concatenates the elements of a list.</summary>
        /// <typeparam name="T">The list type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>A concatenated string containing all elements of the list.</returns>
        public static string Join<T>(this List<T> list, string separator)
        {
            return string.Join(separator, list.ToArray());
        }
    }
}
