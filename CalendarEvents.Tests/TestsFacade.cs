using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CalendarEvents.Tests
{
    public class TestsFacade
    {
        public static class EventsFacade
        {
            public static List<EventModel> BuildEventModelList(int count = 1)
            {
                List<EventModel> resultList = new List<EventModel>(count);

                while (count > 0)
                {
                    resultList.Add(BuildEventModelItem());
                    count--;
                }

                return resultList;
            }            
            public static EventModel BuildEventModelItem()
            {
                return new EventModel()
                {
                    End = DateTime.UtcNow.AddHours(1),
                    Id = Guid.NewGuid(),
                    IsAllDay = false,
                    Start = DateTime.UtcNow.AddMinutes(1),
                    Title = Guid.NewGuid().ToString(),
                    URL = Guid.NewGuid().ToString()
                };
            }
            public static List<EventModelDTO> BuildEventModelDTOList(int count = 1)
            {
                List<EventModelDTO> resultList = new List<EventModelDTO>(count);

                while (count > 0)
                {
                    resultList.Add(BuildEventModelDTOItem());
                    count--;
                }

                return resultList;
            }
            public static EventModelDTO BuildEventModelDTOItem()
            {
                return new EventModelDTO()
                {
                    End = DateTime.UtcNow.AddHours(1),
                    Id = Guid.NewGuid(),
                    IsAllDay = false,
                    Start = DateTime.UtcNow.AddMinutes(1),
                    Base64Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    Description = Guid.NewGuid().ToString(),
                    Details = null,
                    ImagePath = Guid.NewGuid().ToString(),
                    Title = Guid.NewGuid().ToString(),
                    UpdateDate = DateTime.Now,
                    OwnerId = Guid.NewGuid().ToString(),
                    URL = Guid.NewGuid().ToString()
                };
            }
        }

        public static class FilterStatementFacade
        {
            public static IEnumerable<FilterStatement<TEntity>> BuildFilterStatementList<TEntity>(int count = 1)
            {
                List<FilterStatement<TEntity>> resultList = new List<FilterStatement<TEntity>>(count);

                while (count > 0)
                {
                    resultList.Add(BuildFilterStatement<TEntity>());
                    count--;
                }

                return resultList;
            }
            public static FilterStatement<TEntity> BuildFilterStatement<TEntity>()
            {
                return new FilterStatement<TEntity>()
                {
                    Operation = FilterOperation.Equal,
                    PropertyName = "Id",
                    Value = Guid.NewGuid().ToString()
                };
            }
        }

        public static class OrderBytatementFacade
        {
            public static OrderByStatement<TEntity> BuildOrderByStatement<TEntity>()
            {
                return new OrderByStatement<TEntity>()
                {
                    PropertyName = "Id",
                    Direction = OrderByDirection.Desc
                };
            }
        }

        public static class GenericRequestfacade
        {
            public static GetRequest<TEntity> BuildGenericRequest<TEntity>()
            {
                return new GetRequest<TEntity>()
                {
                    Filters = BuildFilterList<TEntity>(),
                    IncludeProperties = "Id,CreateDate"
                };
            }

            public static IEnumerable<FilterStatement<TEntity>> BuildFilterList<TEntity>()
            {
                return new List<FilterStatement<TEntity>>()
                {
                    BuildFilterStatement<TEntity>()
                };
            }

            public static FilterStatement<TEntity> BuildFilterStatement<TEntity>()
            {
                return new FilterStatement<TEntity>()
                {
                    Operation = FilterOperation.Equal,
                    PropertyName = "Id",
                    Value = Guid.NewGuid()
                };
            }
        }
    }
}
