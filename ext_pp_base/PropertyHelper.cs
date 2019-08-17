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
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo(Type t, string name)
        {
            return t.GetField(name);
        }

        /// <summary>
        /// Returns the field info of name in the type t
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the field info of type t using lambda functions
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (FieldInfo)((MemberExpression)body).Member;
            }

            return null;
        }
    }
}