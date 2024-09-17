using System.Linq.Expressions;

namespace Backbone.BuildingBlocks.Application.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expression1, expression2));
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1, expression2));
    }
}
