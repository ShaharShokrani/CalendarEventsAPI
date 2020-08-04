using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {        
        private readonly IGenericService<EventModel> _eventsService;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public EventsController(
            IAuthorizationService authorizationService,
            IGenericService<EventModel> eventsService, 
            IMapper mapper)
        {
            this._eventsService = eventsService ?? throw new ArgumentNullException(nameof(eventsService));
            this._mapper = mapper?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET api/events
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetRequest<EventModelDTO> genericRequestDTO = null)
        {
            try
            {
                if (genericRequestDTO == null)
                    genericRequestDTO = new GetRequest<EventModelDTO>();

                GetRequest<EventModel> genericRequest = this._mapper.Map<GetRequest<EventModel>>(genericRequestDTO);
                ResultHandler<IEnumerable<EventModel>> result = await this._eventsService.Get(genericRequest.Filters, genericRequest.OrderBy, genericRequest.IncludeProperties);
                if (result.Success)
                {
                    IEnumerable<EventModel> list = result.Value as IEnumerable<EventModel>;
                    IEnumerable<EventModelDTO> listDTO = this._mapper.Map<IEnumerable<EventModelDTO>>(list);
                    return Ok(listDTO);
                }
                else
                {
                    return StatusCode(500, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorCode.Unknown);
                //TODO: Log the Exception.
            }
        }

        // GET api/events/c4df7159-2402-4f49-922c-1a2caef02de2
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GET")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }

                ResultHandler<EventModel> result = await this._eventsService.GetById(id);
                if (result.Success)
                {
                    EventModel eventModel = result.Value as EventModel;
                    EventModelDTO eventModelDTO = this._mapper.Map<EventModelDTO>(eventModel);

                    return Ok(eventModelDTO);
                }
                else
                {
                    return StatusCode(500, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorCode.Unknown);
                //TODO: Log the Exception.
            }
        }

        // POST api/events
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //TODO: Use a ModelBinder.
        //public async Task<IActionResult> Post([FromBody] EventPostRequest request = null)

        // POST api/events
        [HttpPost]
        //[Authorize(Policy = "Events.Post")]
        public async Task<IActionResult> Post([FromBody] IEnumerable<EventModelPostDTO> requests = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                
                IEnumerable<EventModel> items = this._mapper.Map<IEnumerable<EventModel>>(requests);

                string ownerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                foreach (var item in items)
                {
                    item.OwnerId = ownerId;
                } 

                ResultHandler rh = await this._eventsService.InsertRange(items);
                if (rh.Success)
                {
                    return CreatedAtAction("Post", items);
                }
                else
                {
                    return StatusCode(500, rh.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorCode.Unknown);
                //TODO: Log the Exception.
            }
        }

        // PUT api/events/
        //[Authorize]
        //[ValidateAntiForgeryToken]
        [HttpPut]
        //[Authorize(Policy = "Events.Put")]
        public async Task<IActionResult> Put([FromBody] EventPutRequest request)
        {
            try
            {                
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ResultHandler<EventModel> getByIdResult = await this._eventsService.GetById(request.Id);
                if (!getByIdResult.Success)
                {
                    return StatusCode(500, getByIdResult.ErrorCode);
                }



                EventModel item = getByIdResult.Value as EventModel;
                item.End = request.End;
                item.IsAllDay = request.IsAllDay;
                item.Title = request.Name;
                item.Start = request.Start;
                item.URL = request.URL;
                item.UpdateDate = DateTime.UtcNow;
                
                ResultHandler result = await this._eventsService.Update(item);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorCode.Unknown);
                //TODO: Log the Exception.
            }
        }

        // DELETE api/events/c4df7159-2402-4f49-922c-1a2caef02de2
        //[Authorize]
        //[ValidateAntiForgeryToken]
        [HttpDelete("{id}")]
        //[Authorize(Policy = "Events.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }

                ResultHandler rh = await this._eventsService.Delete(id);
                if (rh.Success)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(500, rh.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorCode.Unknown);
                //TODO: Log the Exception.
            }
        }
    }
}