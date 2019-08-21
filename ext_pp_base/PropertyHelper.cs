﻿using System;
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
            return GetMemberInfo<PropertyInfo, TValue>(selector);
        }

        private static L GetMemberInfo<L, TValue>(
            Expression<Func<T, TValue>> selector) where L : MemberInfo
        {
            L ret = null;
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            if (body.NodeType == ExpressionType.MemberAccess)
            {
                ret = ((MemberExpression)body).Member as L;
            }
            return ret;

        }

        /// <summary>
        /// Returns the field info of type t using lambda functions
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo<L, TValue>(
            Expression<Func<T, TValue>> selector)
        {

            return GetMemberInfo<FieldInfo, TValue>(selector);
        }
    }
}