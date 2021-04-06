using CalendarEvents.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CalendarEvents.Services
{
    public class MultiCheckboxFilter : IFilter<EventModel>
    {
        private readonly ILogger _logger;
        public MultiCheckboxFilter(ILogger logger)
        {
            this._logger = logger;
        }
        public ResultHandler<Expression<Func<EventModel, bool>>> BuildExpression(FilterStatement<EventModel> filterStatement)
        {
            try
            {
                ExpressionStarter<EventModel> result = PredicateBuilder.New<EventModel>();

                switch (filterStatement.PropertyName)
                {
                    case "Audience":
                        var values = JsonConvert.DeserializeObject<IEnumerable<int>>(filterStatement.ValueJson);
                        foreach (var value in values)
                        {
                            result.Or(x => x.Audience == value);
                        }
                        break;
                    default:
                        throw new NotImplementedException("No filter support for this property");
                }

                return ResultHandler.Ok<Expression<Func<EventModel, bool>>>(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "MultiCheckboxFilter.BuildExpression Failure");
                return ResultHandler.Fail<Expression<Func<EventModel, bool>>>(ex);
            }
        }        
    }
}