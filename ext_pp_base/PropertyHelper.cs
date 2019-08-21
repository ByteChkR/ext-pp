using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ext_pp_base
{
    public static class PropertyHelper
    {
        /// <summary>
        /// Returns the field info of name in the type t
        /// </summary>
        /// <param name="t">Type of the containing class</param>
        /// <param name="name">name of the field</param>
        /// <returns>The field info</returns>
        public static FieldInfo GetFieldInfo(Type t, string name)
        {
            return t.GetField(name);
        }

        /// <summary>
        /// Returns the field info of name in the type t
        /// </summary>
        /// <param name="t">Type of the containing class</param>
        /// <param name="name">name of the property</param>
        /// <returns>The Property info</returns>
        public static PropertyInfo GetPropertyInfo(Type t, string name)
        {
            return t.GetRuntimeProperty(name);
        }
    }

    public static class PropertyHelper<T>
    {

        /// <summary>
        /// Returns the property info of type t using lambda functions
        /// </summary>
        /// <typeparam name="TValue">The type of the property</typeparam>
        /// <param name="selector">The selector for the property</param>
        /// <returns>Property info of the selected property</returns>
        public static PropertyInfo GetPropertyInfo<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            return GetMemberInfo(selector) as PropertyInfo;
        }
        /// <summary>
        /// Returns the member info of type t using lambda functions
        /// </summary>
        /// <typeparam name="TValue">The type of the member</typeparam>
        /// <param name="selector">The selector for the member</param>
        /// <returns>The member info</returns>
        private static MemberInfo GetMemberInfo<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            if (body.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression)body).Member as MemberInfo;
            }
            return null;

        }

        /// <summary>
        /// Returns the field info of type t using lambda functions
        /// </summary>
        /// <typeparam name="TValue">The type of the field</typeparam>
        /// <param name="selector">The selector for the field</param>
        /// <returns>field info of the selected field</returns>
        public static FieldInfo GetFieldInfo<TValue>(
            Expression<Func<T, TValue>> selector)
        {

            return GetMemberInfo(selector) as FieldInfo;
        }
    }
}