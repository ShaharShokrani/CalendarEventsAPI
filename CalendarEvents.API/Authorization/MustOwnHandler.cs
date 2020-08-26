using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
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

        public async Task<string> RequestBody(Stream body)
        {
            var bodyStream = new StreamReader(body);
            return await bodyStream.ReadToEndAsync();
        }

        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MustOwnRequirement<T> requirement)
        {
            if (!context.PendingRequirements.Any(req => req.GetType().Name == typeof(MustOwnRequirement<T>).Name))
                return;

            string requestedBody = await RequestBody(_httpContextAccessor.HttpContext.Request.Body);
            var genericEntity = JsonConvert.DeserializeObject<T>(requestedBody);
            //if (!Guid.TryParse(id.Id, out Guid guid))
            //{
            //    context.Fail();
            //    return;
            //}
            string ownerId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;            

            var isOwnerRH = await this._eventService.IsOwner(genericEntity.Id, ownerId);

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
