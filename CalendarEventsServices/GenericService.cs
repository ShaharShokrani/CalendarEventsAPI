using CalendarEvents.Models;
using CalendarEvents.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CalendarEvents.Services
{
    public interface IGenericService<T> : IGetService<T>, 
                                      IUpdateService<T>,
                                      IInsertService<T>,
                                      IDeleteService
    {
    }

    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            this._repository = repository;
        }

        public ResultService Insert(T obj)
        {
            try
            {
                this._repository.Insert(obj);

                return ResultService.Ok();
            }
            catch (Exception ex)
            {
                return ResultService.Fail(ex);
            }
        }

        public ResultService<IEnumerable<T>> Get(IEnumerable<FilterStatement<T>> filterStatements,
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
                        return ResultService.Fail<IEnumerable<T>>(filtersResult.ErrorCode);
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
                        return ResultService.Fail<IEnumerable<T>>(orderByResult.ErrorCode);
                    }
                    orderBy = orderByResult.Value as Func<IQueryable<T>, IOrderedQueryable<T>>;
                }

                var result = this._repository.Get(filters, orderBy, includeProperties);
                if (result == null)
                {
                    return ResultService.Fail<IEnumerable<T>>(ErrorCode.NotFound);
                }
                return ResultService.Ok<IEnumerable<T>>(result);
            }
            catch (Exception ex)
            {
                return ResultService.Fail<IEnumerable<T>>(ex);
            }
        }

        public ResultService<T> GetById(object id)
        {
            try
            {
                var entity = this._repository.GetById(id);
                if (entity == null)
                {
                    return ResultService.Fail<T>(ErrorCode.NotFound);
                }

                return ResultService.Ok<T>(entity);
            }
            catch (Exception ex)
            {
                return ResultService.Fail<T>(ex);
            }
        }

        public ResultService Delete(object id)
        {
            try
            {
                var entity = this._repository.GetById(id);
                if (entity == null)
                {
                    return ResultService.Fail(ErrorCode.NotFound);
                }
                this._repository.Remove(id);

                return ResultService.Ok();
            }
            catch (Exception ex)
            {
                return ResultService.Fail(ex);
            }
        }

        public ResultService Update(T obj)
        {
            try
            {                
                this._repository.Update(obj);
                return ResultService.Ok(obj);
            }
            catch (Exception ex)
            {
                return ResultService.Fail(ex);
            }
        }        
    }
}
