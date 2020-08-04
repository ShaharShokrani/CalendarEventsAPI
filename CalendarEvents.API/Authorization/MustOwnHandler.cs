using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace CalendarEvents.API.Authorization
{
    public class MustOwnHandler<T> : AuthorizationHandler<MustOwnRequirement<T>> where T : class, IGenericEntity
    {
        private readonly IGenericService<T> _eventService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustOwnHandler(
            IGenericService<T> eventService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MustOwnRequirement<T> requirement)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            var id = _httpContextAccessor.HttpContext.GetRouteValue("id").ToString();
            if (!Guid.TryParse(id, out Guid guid))
            {
                context.Fail();
                return;
            }

            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var isOwnerRH = await this._eventService.IsOwner(guid, ownerId);

            if (!isOwnerRH.Success)
            {
                //TODO: Add logger
                context.Fail();
                return;
            }

            bool isOwner = isOwnerRH.Value;
            if (!isOwnerRH.Value)
            {
                context.Fail();
                return;
            }

            // all checks out
            context.Succeed(requirement);
            return;
        }
    }
}
