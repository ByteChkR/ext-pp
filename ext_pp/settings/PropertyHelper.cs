using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ext_compiler.settings
{
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
                    Logger.Crash(new InvalidOperationException());
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
                    Logger.Crash(new InvalidOperationException());
                    return null;
            }
        }
    }
}