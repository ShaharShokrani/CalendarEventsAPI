using CalendarEvents.Models;
using CalendarEvents.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalendarEvents.Services
{
    public interface IGenericService<T> : IGetService<EventModel>, 
                                        IUpdateService<EventModel>,
                                        IInsertService<EventModel>,
                                        IOwnerService<EventModel>,
                                        IDeleteService
    {
    }

    public class EventService : IGenericService<EventModel>
    {
        private readonly IFiltersService<EventModel> _filtersService;
        private readonly IGenericRepository<EventModel> _repository;
        private readonly ILogger<EventModel> _log;

        public EventService(IFiltersService<EventModel> filtersService, IGenericRepository<EventModel> repository, ILogger<EventModel> log)
        {
            this._filtersService = filtersService?? throw new ArgumentNullException(nameof(filtersService));
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._log = log ?? throw new ArgumentNullException(nameof(log));
        }

        //public bool CheckIsApproval(int id)
        //{
        //    try
        //    {
        //        var order = this._dataAccess.GetOrder(id);
        //        // Compute if the order is approved 
        //        bool response = CheckIfApproved(order);

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.LogError(ex, "CheckIsApproval Failed");
        //        return false;
        //    }
        //}
        public async Task<ResultHandler> InsertRange(IEnumerable<EventModel> items)
        {
            try
            {
                await this._repository.InsertRange(items);
                return ResultHandler.Ok();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed inserting object to DB");
                return ResultHandler.Fail(ex);
            }
        }
        public ResultHandler<IAsyncEnumerable<EventModel>> Get(IEnumerable<FilterStatement<EventModel>> filterStatements,
                                                OrderByStatement<EventModel> orderByStatement = null,
                                                string includeProperties = "")
        {
            try
            {
                IEnumerable<Expression<Func<EventModel, bool>>> filters = null;
                if (filterStatements != null)
                {
                    var filtersResult = this._filtersService.BuildExpressions(filterStatements);
                    if (!filtersResult.Success)
                    {
                        return ResultHandler.Fail<IAsyncEnumerable<EventModel>>(filtersResult.ErrorCode);
                    }
                    filters = filtersResult.Value;
                }

                Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>> orderBy = null;
                if (orderByStatement != null)
                {
                    var orderByService = new OrderByService();
                    var orderByResult = orderByService.GetOrderBy<EventModel>(orderByStatement);
                    if (!orderByResult.Success)
                    {
                        return ResultHandler.Fail<IAsyncEnumerable<EventModel>>(orderByResult.ErrorCode);
                    }
                    orderBy = orderByResult.Value as Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>>;
                }

                IAsyncEnumerable<EventModel> result = this._repository.Get(filters, orderBy, includeProperties);

                return ResultHandler.Ok(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get objects from DB");
                return ResultHandler.Fail<IAsyncEnumerable<EventModel>>(ex);
            }
        }
        public async Task<ResultHandler<EventModel>> GetById(Guid id)
        {
            try
            {
                var entity = await this._repository.GetById(id);
                if (entity == null)
                    return ResultHandler.Fail<EventModel>(ErrorCode.EntityNotFound);

                return ResultHandler.Ok<EventModel>(entity);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get object with id: {0} from DB", id);
                return ResultHandler.Fail<EventModel>(ex);
            }
        }

        public async Task<ResultHandler> Delete(Guid id)
        {
            try
            {
                var entity = await this._repository.GetById(id);
                if (entity == null)
                {
                    return ResultHandler.Fail(ErrorCode.EntityNotFound, "Entity not found");
                }
                await this._repository.Remove(entity);

                return ResultHandler.Ok();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to  delete object with id: {0} from DB", id);
                return ResultHandler.Fail(ex);
            }
        }

        public async Task<ResultHandler> Update(EventModel obj)
        {
            try
            {                
                await this._repository.Update(obj);
                return ResultHandler.Ok(obj);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to update object");
                return ResultHandler.Fail(ex);
            }
        }

        public async Task<ResultHandler<bool>> IsOwner(Guid id, string ownerId)
        {
            try
            {
                var isOwner = await this._repository.IsOwner(id, ownerId);
                if (isOwner)
                    return ResultHandler.Ok<bool>(isOwner);
                else
                    return ResultHandler.Fail<bool>(ErrorCode.Unauthorized);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get object with id: {0} from DB", id);
                return ResultHandler.Fail<bool>(ex);
            }
        }

        public Task<ResultHandler> Insert(EventModel obj)
        {
            throw new NotImplementedException();
        }
    }
}
