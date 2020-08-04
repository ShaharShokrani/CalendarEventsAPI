using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel;


namespace CalendarEvents.Services
{
    /// <summary>
    /// Defines a filter from which a expression will be built.
    /// </summary>
    public interface IFiltersService<T>
    {
        /// <summary>
        /// Builds a LINQ expression based upon the statements included in this filter.
        /// </summary>
        /// <returns></returns>
        ResultService<Expression<Func<T, bool>>> BuildExpression();
    }

    public class FiltersService<T> : IFiltersService<T>
    {
        /// <summary>
        /// Group of statements that compose this filter.
        /// </summary>
        private readonly IEnumerable<FilterStatement<T>> _filterStatements = null;
        public FiltersService(IEnumerable<FilterStatement<T>> filterStatements)
        {
            this._filterStatements = filterStatements ?? throw new ArgumentNullException(nameof(filterStatements));
        }

        public ResultService<Expression<Func<T, bool>>> BuildExpression()
        {
            try
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
                Expression finalExpression = Expression.Constant(true);

                foreach (var statement in _filterStatements)
                {
                    if (!statement.IsValid)
                    {
                        return ResultService.Fail<Expression<Func<T, bool>>>(ErrorCode.EntityNotValid);
                    }

                    Type propType = typeof(T).GetProperty(statement.PropertyName).PropertyType;
                    TypeConverter converter = TypeDescriptor.GetConverter(propType);
                    object convertedObject = converter.ConvertFrom(statement.Value);
                    
                    var member = Expression.Property(parameterExpression, statement.PropertyName);
                    var constant = Expression.Constant(convertedObject);
                    Expression expression = null;

                    switch (statement.Operation)
                    {
                        case FilterOperation.Equal:
                            expression = Expression.Equal(member, constant);
                            break;
                        case FilterOperation.Contains:
                            throw new NotImplementedException();
                        //break;
                        case FilterOperation.StartsWith:
                            throw new NotImplementedException();
                        //break;
                        case FilterOperation.EndsWith:
                            throw new NotImplementedException();
                        //break;
                        case FilterOperation.NotEqual:
                            expression = Expression.NotEqual(member, constant);
                            break;
                        case FilterOperation.GreaterThan:
                            expression = Expression.GreaterThan(member, constant);
                            break;
                        case FilterOperation.GreaterThanOrEqual:
                            expression = Expression.GreaterThanOrEqual(member, constant);
                            break;
                        case FilterOperation.LessThan:
                            expression = Expression.LessThan(member, constant);
                            break;
                        case FilterOperation.LessThanOrEqual:
                            expression = Expression.LessThanOrEqual(member, constant);
                            break;
                        default:
                            throw new NotImplementedException();
                            //break;
                    };

                    finalExpression = Expression.AndAlso(finalExpression, expression);
                }

                Expression<Func<T, bool>> result = Expression.Lambda<Func<T, bool>>(finalExpression, parameterExpression);
                return ResultService.Ok<Expression<Func<T, bool>>>(result);
            }
            catch (Exception ex)
            {
                return ResultService.Fail<Expression<Func<T, bool>>>(ex);
            }
        }
    }
}