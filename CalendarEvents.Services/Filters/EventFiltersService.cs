using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

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
        ResultHandler<IEnumerable<Expression<Func<T, bool>>>> BuildExpressions(IEnumerable<FilterStatement<T>> filterStatements);
    }

    public interface IFilter<T>
    {
        ResultHandler<Expression<Func<T, bool>>> BuildExpression(FilterStatement<T> filterStatement);
    }

    public class EventFiltersService : IFiltersService<EventModel>
    {
        private readonly EventFilterResolver _eventFilterResolver;
        private readonly ILogger _logger;
        public EventFiltersService(EventFilterResolver eventFilterResolver, ILogger logger)
        {
            this._eventFilterResolver = eventFilterResolver;
            this._logger = logger;
        }
        public ResultHandler<IEnumerable<Expression<Func<EventModel, bool>>>> BuildExpressions(IEnumerable<FilterStatement<EventModel>> filterStatements)
        {
            try
            {
                List<Expression<Func<EventModel, bool>>> result = new List<Expression<Func<EventModel, bool>>>();
                foreach (var filterStatement in filterStatements)
                {
                    IFilter<EventModel> filter = this._eventFilterResolver(filterStatement.FilterType);
                    ResultHandler<Expression<Func<EventModel, bool>>> expressionResult = filter.BuildExpression(filterStatement);
                    if (!expressionResult.Success)
                    {
                        this._logger.LogError(expressionResult.ErrorMessage);
                        return ResultHandler.Fail<IEnumerable<Expression<Func<EventModel, bool>>>>(expressionResult.ErrorCode);
                    }

                    result.Add(expressionResult.Value);
                }                
                return ResultHandler.Ok<IEnumerable<Expression<Func<EventModel, bool>>>>(result);
            }
            catch (Exception ex)
            {
                return ResultHandler.Fail<IEnumerable<Expression<Func<EventModel, bool>>>>(ex);
            }
        }
    }
}