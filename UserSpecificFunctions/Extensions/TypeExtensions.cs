using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace UserSpecificFunctions.Extensions
{
    /// <summary>Provides extension methods for the <see cref="Type"/> class.</summary>
    public static class TypeExtensions
    {
        /// <summary>Invokes a 'private static' method.</summary>
        /// <typeparam name="T">The method's return type.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="name">The method name.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><typeparamref name="T"/></returns>
        public static T InvokePrivateMethod<T>(this Type type, string name, params object[] args)
        {
            MethodInfo methodInfo = type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
            return (T)methodInfo.Invoke(null, args);
        }
    }
}
