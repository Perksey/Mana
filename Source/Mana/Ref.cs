using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mana
{
    // Teo u legend
    // https://blog.tchatzigiannakis.com/creating-a-ref-alternative-in-c-sharp/

    public interface IRef
    {
    }
    
    public struct Ref
    {
        public static Ref<T> Of<T>(Expression<Func<T>> expr)
        {
            return Ref<T>.Of(expr);
        }
    }
    
    public struct Ref<T> : IRef
    {
        public T Value
        {
            get => _get();
            set => _set(value);
        }

        private readonly Func<T> _get;
        private readonly Action<T> _set;

        public Func<T> Getter => _get;
        public Action<T> Setter => _set;

        public Ref(Func<T> get, Action<T> set)
        {
            _get = get;
            _set = set;
        }

        public static Ref<T> Of(Expression<Func<T>> expr)
        {
            var get = expr.Compile();
            var param = new[] { Expression.Parameter(typeof(T)) };
            var op = CreateSetOperation(expr.Body, param[0]);
            var act = Expression.Lambda<Action<T>>(op, param);
            var set = act.Compile();
            return new Ref<T>(get, set);
        }

        internal static Expression CreateSetOperation(Expression expr, Expression param)
        {
            switch (expr)
            {
                case BinaryExpression binaryExpr:
                    return CreateSetOperation(binaryExpr, param);
                case MemberExpression memberExpr:
                    return CreateSetOperation(memberExpr, param);
                case MethodCallExpression callExpr:
                    return CreateSetOperation(callExpr, param);
                default:
                    throw new NotSupportedException("This kind of expression is not supported.");
            }
        }

        internal static Expression CreateSetOperation(MemberExpression expr, Expression param)
        {
            PropertyInfo propertyInfo = expr.Member as PropertyInfo;
            
            if (propertyInfo != null && !propertyInfo.CanWrite)
            {
                return Expression.Throw(Expression.New(typeof(InvalidOperationException)));
            }
            
            return Expression.Assign(expr, param);
        }

        internal static Expression CreateSetOperation(MethodCallExpression expr, Expression param)
        {
            var setterName = expr.Method.Name.Replace("get_", "set_");
            var setter = expr.Method.DeclaringType?.GetMethod(setterName);
            
            if (setter == null)
            {
                return Expression.Throw(Expression.New(typeof(InvalidOperationException)));
            }

            return Expression.Call(expr.Object, setter, expr.Arguments.Concat(new[] { param }));
        }
    }
}