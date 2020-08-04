using AutoMapper;
using CalendarEvents.Models;

namespace CalendarEvents
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<EventModel, EventModelDTO>();
            CreateMap<EventModelDTO, EventModel>();

            CreateMap<EventModel, EventModelPostDTO>();
            CreateMap<EventModelPostDTO, EventModel>();

            CreateMap<EventModel, EventPutRequest>();
            CreateMap<EventPutRequest, EventModel>();

            CreateMap<GetRequest<EventModelDTO>, GetRequest<EventModel>>();
            CreateMap<GetRequest<EventModel>, GetRequest<EventModelDTO>>();
        }
    }
}
