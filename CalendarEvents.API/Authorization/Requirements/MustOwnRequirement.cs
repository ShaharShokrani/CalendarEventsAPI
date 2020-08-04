using CalendarEvents.Models;
using Microsoft.AspNetCore.Authorization;

namespace CalendarEvents.API.Authorization
{
    public class MustOwnRequirement<T> : IAuthorizationRequirement where T : class , IGenericEntity
    {
        public MustOwnRequirement()
        {
        }
    }
}
