using AutoMapper;
using CalendarEvents.Common;
using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalendarEvents.Tests
{
    public class TestsFacade
    {
        public static class EventsFacade
        {
            public static IEnumerable<EventModel> BuildEventModels(int count = 1)
            {
                List<EventModel> resultList = new List<EventModel>(count);

                while (count > 0)
                {
                    resultList.Add(BuildEventModel());
                    count--;
                }

                return resultList;
            }            
            private static EventModel BuildEventModel()
            {
                return new EventModel()
                {
                    End = DateTime.UtcNow.AddHours(1),
                    Id = Guid.NewGuid(),
                    IsAllDay = false,
                    Start = DateTime.UtcNow.AddMinutes(1),
                    Title = Guid.NewGuid().ToString(),
                    URL = Guid.NewGuid().ToString(),
                    Base64Id = Utils.Base64Encode(Guid.NewGuid().ToString()),
                    CreateDate = DateTime.UtcNow,
                    Description = Guid.NewGuid().ToString(),
                    Details = JsonConvert.SerializeObject(new { }),
                    ImagePath = Guid.NewGuid().ToString(),
                    OwnerId = Guid.NewGuid().ToString(),
                    UpdateDate = DateTime.UtcNow
                };
            }

            public static async IAsyncEnumerable<EventModel> BuildIAsyncEnumerable(IEnumerable<EventModel> eventModels = null)
            {
                if (eventModels == null)
                    eventModels = BuildEventModels();

                foreach (EventModel eventModel in eventModels)
                {
                    await Task.CompletedTask;
                    yield return eventModel;
                }
            }

            public static List<EventModelPostDTO> BuildEventModelPostsDTOs(IEnumerable<EventModel> eventModels = null)
            {
                if (eventModels == null)
                    eventModels = BuildEventModels();

                List<EventModelPostDTO> result = new List<EventModelPostDTO>();

                foreach (var eventModel in eventModels)
                {
                    result.Add(BuildEventModelPostDTO(eventModel));
                }

                return result;
            }

            public static async Task<List<EventModelDTO>> BuildEventModelDTOs(IAsyncEnumerable<EventModel> eventModels)
            {
                if (eventModels == null)
                    eventModels = BuildIAsyncEnumerable();

                List<EventModelDTO> result = new List<EventModelDTO>();

                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();

                await foreach (var eventModel in eventModels)
                {
                    result.Add(BuildEventModelDTO(eventModel));
                }

                return result;
            }
            private static EventModelDTO BuildEventModelDTO(EventModel eventModel)
            {
                return new EventModelDTO()
                {
                    End = eventModel.End,
                    Id = eventModel.Id,
                    IsAllDay = eventModel.IsAllDay,
                    Start = eventModel.Start,
                    Base64Id = eventModel.Base64Id,
                    CreateDate = eventModel.CreateDate,
                    Description = eventModel.Description,
                    Details = eventModel.Details,
                    ImagePath = eventModel.ImagePath,
                    Title = eventModel.Title,
                    UpdateDate = eventModel.UpdateDate,
                    OwnerId = eventModel.OwnerId,
                    URL = eventModel.URL                    
                };
            }
            public static EventModelPostDTO BuildEventModelPostDTO(EventModel eventModel)
            {
                return new EventModelPostDTO()
                {
                    End = eventModel.End,
                    IsAllDay = eventModel.IsAllDay,
                    Start = eventModel.Start,
                    Title = eventModel.Title,
                    URL = eventModel.URL,
                    Description = eventModel.Description,
                    Details = eventModel.Details,
                    ImagePath = eventModel.ImagePath                    
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
            public static FilterStatement<TEntity> BuildFilterStatement<TEntity>(
                FilterOperation filterOperation = FilterOperation.Undefined,
                string propertyName = null,
                object value = null)
            {
                if (filterOperation == FilterOperation.Undefined)
                    filterOperation = FilterOperation.Equal;
                if (string.IsNullOrEmpty(propertyName))
                    propertyName = "Id";
                if (value == null)
                    value = Guid.NewGuid().ToString();

                return new FilterStatement<TEntity>()
                {
                    Operation = filterOperation,
                    PropertyName = propertyName,
                    Value = value.ToString()
                };
            }
        }

        public static class OrderBytatementFacade
        {
            public static OrderByStatement<TEntity> BuildOrderByStatement<TEntity>()
            {
                return new OrderByStatement<TEntity>()
                {
                    PropertyName = "OwnerId",
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
                    Value = Guid.NewGuid().ToString()
                };
            }
        }
    }
}
