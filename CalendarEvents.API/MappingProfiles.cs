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
            
            CreateMap<SearchRequest<EventModelDTO>, SearchRequest<EventModel>>();
            CreateMap<SearchRequest<EventModel>, SearchRequest<EventModelDTO>>();
            CreateMap<SearchRequest<EventModel>, SearchRequest<EventModelDTO>>();

            CreateMap<FilterStatement<EventModel>, FilterStatement<EventModelDTO>>();             
            CreateMap<FilterStatement<EventModelDTO>, FilterStatement<EventModel>>();

            CreateMap<UserModel, UserListDTO>();
            CreateMap<UserModel, UserDetailedDTO>();
            CreateMap<UserUpdateDTO, UserModel>();
            CreateMap<UserRegisterDTO, UserModel>();
        }
    }
}
