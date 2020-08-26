using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CalendarEvents.Services
{
    public interface IUserService
    {
        Guid OwnerId { get; }
    }
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ScrapingService> _log;

        public UserService(IHttpContextAccessor httpContextAccessor, ILogger<ScrapingService> log)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._log = log;
        }

        public Guid OwnerId => this.GetOwnerId();

        private Guid GetOwnerId()
        {
            var ownerId = this._httpContextAccessor.HttpContext
                              .User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Guid.TryParse(ownerId, out var id) ? id : Guid.Empty;
        }       
    }
}