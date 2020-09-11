using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalendarEvents.Controllers
{
    [Route("api/[controller]/[action]")]    
    [ApiController]
    public class EventsController : ControllerBase
    {        
        private readonly IGenericService<EventModel> _eventsService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ILogger<EventsController> _log;

        public EventsController(
            IUserService userService,
            IGenericService<EventModel> eventsService, 
            IMapper mapper,
            ILogger<EventsController> log)
        {
            this._eventsService = eventsService ?? throw new ArgumentNullException(nameof(eventsService));
            this._mapper = mapper?? throw new ArgumentNullException(nameof(mapper));
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._log = log ?? throw new ArgumentNullException(nameof(log));
        }

        // GET api/events
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Search([FromBody]GetRequest<EventModelDTO> genericRequestDTO = null)
        {
            try
            {
                if (genericRequestDTO == null)
                    genericRequestDTO = new GetRequest<EventModelDTO>();

                GetRequest<EventModel> genericRequest = this._mapper.Map<GetRequest<EventModel>>(genericRequestDTO);
                var getResultHandler = this._eventsService.Get(genericRequest.Filters, genericRequest.OrderBy, genericRequest.IncludeProperties);
                if (getResultHandler.Success)
                {
                    List<EventModelDTO> result = new List<EventModelDTO>();
                    await foreach (EventModel eventModel in getResultHandler.Value)
                    {
                        EventModelDTO eventModelDTO = this._mapper.Map<EventModelDTO>(eventModel);
                        result.Add(eventModelDTO);
                    }
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, getResultHandler.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                this._log.LogError(ex, "EventsController.Get");
                return StatusCode(500, ErrorCode.Unknown);                
            }
        }

        // GET api/events)
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(ModelState);
                }

                ResultHandler<EventModel> result = await this._eventsService.GetById(id);
                if (result.Success)
                {
                    EventModel eventModel = result.Value as EventModel;
                    EventModelDTO eventModelDTO = this._mapper.Map<EventModelDTO>(eventModel);

                    return Ok(eventModelDTO);
                }
                else if (result.ErrorCode == ErrorCode.NotFound)
                    return NotFound($"Not found entity with Id: {id}");
                else
                    return StatusCode(500, result.ErrorCode);
            }
            catch (Exception ex)
            {
                this._log.LogError(ex, "EventsController.Get");
                return StatusCode(500, ErrorCode.Unknown);                
            }
        }

        // POST api/events
        //[ValidateAntiForgeryToken]
        //TODO: Use a ModelBinder.
        //public async Task<IActionResult> Post([FromBody] EventPostRequest request = null)

        // POST api/events
        [HttpPost]
        [Authorize(Policy = "Events.Insert")]
        public async Task<IActionResult> Insert(IEnumerable<EventModelPostDTO> requests = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                IEnumerable<EventModel> items = this._mapper.Map<IEnumerable<EventModel>>(requests);
                
                string ownerId = this._userService.OwnerId.ToString();
                foreach (var item in items)
                {
                    item.OwnerId = ownerId;
                }

                ResultHandler rh = await this._eventsService.InsertRange(items);
                if (rh.Success)
                {
                    IEnumerable<EventModelDTO> listDTO = this._mapper.Map<IEnumerable<EventModelDTO>>(items);
                    return CreatedAtAction(nameof(Insert), listDTO);
                }
                else
                {
                    return StatusCode(500, rh.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                this._log.LogError(ex, "EventsController.Post");
                return StatusCode(500, ErrorCode.Unknown);
            }
        }
        
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Policy = "Events.Update")]
        public async Task<IActionResult> Update(EventPutRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                {
                    return BadRequest(request);
                }

                ResultHandler<EventModel> getByIdResult = await this._eventsService.GetById(request.Id);
                if (!getByIdResult.Success)
                {
                    return StatusCode(500, getByIdResult.ErrorCode);
                }

                EventModel item = getByIdResult.Value as EventModel;

                string ownerId = this._userService.OwnerId.ToString();
                if (!item.OwnerId.Equals(ownerId))
                    return Unauthorized(request);

                item.End = request.End;
                item.IsAllDay = request.IsAllDay;
                item.Title = request.Name;
                item.Start = request.Start;
                item.URL = request.URL;
                item.Base64Id = request.Base64Id;
                item.Description = request.Description;
                item.Details = request.Details;
                item.ImagePath = request.ImagePath;
                item.UpdateDate = DateTime.UtcNow;
                
                ResultHandler result = await this._eventsService.Update(item);
                if (result.Success)
                {
                    EventModelDTO eventModelDTO = this._mapper.Map<EventModelDTO>(item);
                    return Ok(eventModelDTO);
                }
                else
                {
                    return StatusCode(500, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                this._log.LogError(ex, "EventsController.Put");
                return StatusCode(500, ErrorCode.Unknown);                
            }
        }
        
        //[ValidateAntiForgeryToken]        
        [HttpPost]
        [Authorize(Policy = "Events.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {                
                if (id == Guid.Empty)
                {
                    ModelState.AddModelError(nameof(id), "Is empty.");
                    return BadRequest(ModelState);
                }

                ResultHandler rh = await this._eventsService.Delete(id);
                if (rh.Success)
                {
                    return Ok(id);
                }
                else
                {
                    return StatusCode(500, rh.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                this._log.LogError(ex, "EventsController.Delete");
                return StatusCode(500, ErrorCode.Unknown);                
            }
        }
    }
}