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
    public interface IGenericService<T> : IGetService<T>, 
                                        IUpdateService<T>,
                                        IInsertService<T>,
                                        IOwnerService<T>,
                                        IDeleteService
    {
    }

    public class GenericService<T> : IGenericService<T> where T : class, IGenericEntity
    {
        private readonly IGenericRepository<T> _repository;
        private readonly ILogger<GenericService<T>> _log;

        public GenericService(IGenericRepository<T> repository, ILogger<GenericService<T>> log)
        {
            this._repository = repository;
            this._log = log;
        }

        public async Task<ResultHandler> Insert(T obj)
        {
            try
            {
                await this._repository.Add(obj);
                return ResultHandler.Ok();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed inserting object to DB");
                return ResultHandler.Fail(ex);
            }
        }
        public async Task<ResultHandler> InsertRange(IEnumerable<T> items)
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
        public async Task<ResultHandler<IEnumerable<T>>> Get(IEnumerable<FilterStatement<T>> filterStatements,
                                                OrderByStatement<T> orderByStatement = null,
                                                string includeProperties = "")
        {
            try
            {
                Expression<Func<T, bool>> filters = null;
                if (filterStatements != null && filterStatements.Any())
                {                    
                    var filterService = new FiltersService<T>(filterStatements);
                    var filtersResult = filterService.BuildExpression();
                    if (!filtersResult.Success)
                    {
                        return ResultHandler.Fail<IEnumerable<T>>(filtersResult.ErrorCode);
                    }
                    filters = filtersResult.Value as Expression<Func<T, bool>>;
                }

                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null;
                if (orderByStatement != null)
                {
                    var orderByService = new OrderByService();
                    var orderByResult = orderByService.GetOrderBy<T>(orderByStatement);
                    if (!orderByResult.Success)
                    {
                        return ResultHandler.Fail<IEnumerable<T>>(orderByResult.ErrorCode);
                    }
                    orderBy = orderByResult.Value as Func<IQueryable<T>, IOrderedQueryable<T>>;
                }

                var result = await this._repository.Get(filters, orderBy, includeProperties);

                return ResultHandler.Ok<IEnumerable<T>>(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get objects from DB");
                return ResultHandler.Fail<IEnumerable<T>>(ex);
            }
        }
        public async Task<ResultHandler<T>> GetById(Guid id)
        {
            try
            {
                var entity = await this._repository.GetById(id);
                return ResultHandler.Ok<T>(entity);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get object with id: {0} from DB", id);
                return ResultHandler.Fail<T>(ex);
            }
        }

        public async Task<ResultHandler> Delete(Guid id)
        {
            try
            {
                var entity = await this._repository.GetById(id);
                if (entity == null)
                {
                    return ResultHandler.Fail(ErrorCode.NotFound);
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

        public async Task<ResultHandler> Update(T obj)
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
                var entity = await this._repository.IsOwner(id, ownerId);
                return ResultHandler.Ok<bool>(entity);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to get object with id: {0} from DB", id);
                return ResultHandler.Fail<bool>(ex);
            }
        }
    }
}
