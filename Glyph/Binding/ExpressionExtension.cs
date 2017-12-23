using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Glyph.Binding
{
    static public class ExpressionExtension
    {
        static public Expression<Action<TInstance, TProperty>> ToSetter<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> expression)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var property = (PropertyInfo)memberExpression.Member;

            ParameterExpression valueParameter = Expression.Parameter(typeof(TProperty), "value");
            MethodCallExpression call = Expression.Call(memberExpression.Expression, property.GetSetMethod(), valueParameter);
            
            return Expression.Lambda<Action<TInstance, TProperty>>(call, expression.Parameters[0], valueParameter);
        }
    }
}