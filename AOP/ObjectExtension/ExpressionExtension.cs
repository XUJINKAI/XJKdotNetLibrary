using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using XJKdotNetLibrary.MethodWrapper;

namespace XJKdotNetLibrary.ObjectExtension
{
    public static class ExpressionExtension
    {
        public static MethodCallInfo ToMethodCall(this LambdaExpression expr)
        {
            switch (expr.Body)
            {
                case MethodCallExpression methodcall:
                    return methodcall.ToMethodCall();
                case UnaryExpression unary:
                    switch (unary.Operand)
                    {
                        case MethodCallExpression mc1:
                            return mc1.ToMethodCall();
                        default:
                            throw new NotSupportedException(unary.Operand.GetType().ToString());
                    }
                default:
                    throw new NotSupportedException(expr.Body.GetType().ToString());
            }
        }

        public static MethodCallInfo ToMethodCall(this MethodCallExpression methodCallExpression)
        {
            var result = new MethodCallInfo
            {
                Name = methodCallExpression.Method.Name,
                Args = new List<object>()
            };
            foreach (var exp in methodCallExpression.Arguments)
            {
                result.Args.Add(exp.Evaluate());
            }
            return result;
        }

        public static object Evaluate(this Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;
                case ExpressionType.New:
                    var newexpr = (NewExpression)expr;
                    var arguments = newexpr.Arguments.Select(Evaluate).ToArray();
                    return newexpr.Constructor.Invoke(arguments);
                case ExpressionType.ListInit:
                    var listexpr = (ListInitExpression)expr;
                    var listobj = Evaluate(listexpr.NewExpression);
                    foreach (var element in listexpr.Initializers)
                    {
                        element.AddMethod.Invoke(listobj, element.Arguments.Select(Evaluate).ToArray());
                    }
                    return listobj;
                case ExpressionType.MemberAccess:
                    var me = (MemberExpression)expr;
                    object target = Evaluate(me.Expression);
                    switch (me.Member.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)me.Member).GetValue(target);
                        case MemberTypes.Property:
                            return ((PropertyInfo)me.Member).GetValue(target, null);
                        default:
                            throw new NotSupportedException(me.Member.MemberType.ToString());
                    }
                default:
                    throw new NotSupportedException(expr.NodeType.ToString());
            }
        }
    }
}
