using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CalendarEvents.Services
{
    public interface IOrderByService
    {
        ResultHandler<Func<IQueryable<T>, IOrderedQueryable<T>>> GetOrderBy<T>(OrderByStatement<T> orderByStatement);
    }

    public class OrderByService : IOrderByService
    {
        public OrderByService()
        {

        }

        public ResultHandler<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> GetOrderBy<TEntity>(OrderByStatement<TEntity> orderByStatement)
        {
            try
            {
                if (orderByStatement == null ||
                    !orderByStatement.IsValid)
                {
                    return ResultHandler.Fail<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>(ErrorCode.ValidationError);
                }

                //1. Detemine method name.
                string methodName = orderByStatement.Direction == OrderByDirection.Asc ? "OrderBy" : "OrderByDescending";

                //2. Build a outer expression
                Type typeQueryable = typeof(IQueryable<TEntity>);
                ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p"); //p
                LambdaExpression outerLambdaExpression = Expression.Lambda(argQueryable, argQueryable); // p => p
                Expression outerExpression = outerLambdaExpression.Body;

                //3. Build a TEntity parameter expression
                Type type = typeof(TEntity);
                ParameterExpression arg = Expression.Parameter(type, "x");

                //4. Build a Memeber expression
                PropertyInfo pi = type.GetProperty(orderByStatement.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                MemberExpression expr = Expression.Property(arg, pi);
                Type memberType = pi.PropertyType;

                //5. Build inner expression:
                LambdaExpression lambda = Expression.Lambda(expr, arg);
                Expression innerExpression = Expression.Quote(lambda);

                MethodCallExpression resultExp = Expression.Call(typeof(Queryable),
                                                                methodName, 
                                                                new Type[] { typeof(TEntity), memberType }, 
                                                                outerExpression, innerExpression);

                var finalLambda = Expression.Lambda(resultExp, argQueryable);

                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> result = (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
                return ResultHandler.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultHandler.Fail<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>(ex);
            }
        }
    }
}
