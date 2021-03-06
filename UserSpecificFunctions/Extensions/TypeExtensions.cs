﻿using System;
using System.Reflection;

namespace UserSpecificFunctions.Extensions
{
	/// <summary>
	/// Provides extension methods for Types.
	/// </summary>
    public static class TypeExtensions
    {
		/// <summary>
		/// Invokes a `private static` method.
		/// </summary>
		/// <typeparam name="T">The method's return type.</typeparam>
		/// <param name="type">The type.</param>
		/// <param name="name">The method's name.</param>
		/// <param name="args">The arguments.</param>
		/// <returns><typeparamref name="T"/></returns>
        public static T CallMethod<T>(this Type type, string name, params object[] args)
        {
            MethodInfo methodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
            return (T)methodInfo.Invoke(null, args);
        }
    }
}
