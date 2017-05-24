using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dolkens.Framework
{
    public class Bitwise
    {
        public readonly Func<Object, Object> NOT;
        public readonly Func<Object, Object, Object> OR;
        public readonly Func<Object, Object, Object> AND;
        public readonly Func<Object, Object, Object> XOR;
        public readonly Func<Object, Object, Boolean> EQUALS;

        public Bitwise(Type underlyingType)
        {
            if (underlyingType.IsEnum) underlyingType = Enum.GetUnderlyingType(underlyingType);

            ParameterExpression param1 = Expression.Parameter(typeof(Object), "left");
            ParameterExpression param2 = Expression.Parameter(typeof(Object), "right");

            Expression left = Expression.Convert(param1, underlyingType);
            Expression right = Expression.Convert(param2, underlyingType);

            NOT = Expression.Lambda<Func<Object, Object>>(Expression.Convert(Expression.Not(left), typeof(Object)), param1).Compile();
            OR = Expression.Lambda<Func<Object, Object, Object>>(Expression.Convert(Expression.Or(left, right), typeof(Object)), param1, param2).Compile();
            AND = Expression.Lambda<Func<Object, Object, Object>>(Expression.Convert(Expression.And(left, right), typeof(Object)), param1, param2).Compile();
            XOR = Expression.Lambda<Func<Object, Object, Object>>(Expression.Convert(Expression.ExclusiveOr(left, right), typeof(Object)), param1, param2).Compile();
            EQUALS = Expression.Lambda<Func<Object, Object, Boolean>>(Expression.Equal(left, right), param1, param2).Compile();
        }
    }

    public class Bitwise<TBase> where TBase : struct, IComparable, IFormattable, IConvertible
    {
        public static readonly Func<TBase, TBase> NOT;
        public static readonly Func<TBase, TBase, TBase> OR;
        public static readonly Func<TBase, TBase, TBase> AND;
        public static readonly Func<TBase, TBase, TBase> XOR;
        public static readonly Func<TBase, TBase, Boolean> EQUALS;

        static Bitwise()
        {
            var underlyingType = typeof(TBase);
            if (underlyingType.IsEnum) underlyingType = Enum.GetUnderlyingType(underlyingType);

            ParameterExpression param1 = Expression.Parameter(underlyingType, "left");
            ParameterExpression param2 = Expression.Parameter(underlyingType, "right");

            Expression left = Expression.Convert(param1, typeof(TBase));
            Expression right = Expression.Convert(param2, typeof(TBase));

            NOT = Expression.Lambda<Func<TBase, TBase>>(Expression.Not(left), param1).Compile();
            OR = Expression.Lambda<Func<TBase, TBase, TBase>>(Expression.Or(left, right), param1, param2).Compile();
            AND = Expression.Lambda<Func<TBase, TBase, TBase>>(Expression.And(left, right), param1, param2).Compile();
            XOR = Expression.Lambda<Func<TBase, TBase, TBase>>(Expression.ExclusiveOr(left, right), param1, param2).Compile();
            EQUALS = Expression.Lambda<Func<TBase, TBase, Boolean>>(Expression.Equal(left, right), param1, param2).Compile();
        }
    }
}
