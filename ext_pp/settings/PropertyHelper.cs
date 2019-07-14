using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ext_pp.settings
{
    public static class PropertyHelper
    {
        public static FieldInfo GetFieldInfo(Type t, string name)
        {
            return t.GetField(name);
        }

        public static PropertyInfo GetPropertyInfo(Type t, string name)
        {
            return t.GetRuntimeProperty(name);
        }
    }

    public static class PropertyHelper<T>
    {
        
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

        public static FieldInfo GetFieldInfo<TValue>(
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
                    return (FieldInfo)((MemberExpression)body).Member;
                default:
                    return null;
            }
        }
    }
}