using CalendarEvents.Models;
using Autofac.Extras.Moq;
using NUnit.Framework;
using System.Collections.Generic;
using CalendarEvents.Tests;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace CalendarEvents.Services.Tests
{
    public class OrderByServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        //1. Is Ok
        //2. throws exception
        //3. propertyName is not valid
        //4. Value is not convertable to propertyName
        //5. Operation is not valid.


        //TODO
        [Test] public void GetOrderBy_WhenCalled_ShouldReturnOk()
        {
            //Arrange
            OrderByStatement<EventModel> orderByStatement = TestsFacade.OrderBytatementFacade.BuildOrderByStatement<EventModel>();
            OrderByService service = new OrderByService();

            //Act
            ResultHandler<Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>>> result = service.GetOrderBy<EventModel>(orderByStatement);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResultHandler<Func<IQueryable<EventModel>,IOrderedQueryable<EventModel>>>>(result);
            Assert.IsTrue(result.Success);
        }
        [Test] public void GetOrderBy_WhenOrderByStatementNull_ShouldReturnFail()
        {
            //Arrange
            OrderByStatement<EventModel> orderByStatement = null;            

            OrderByService service = new OrderByService();

            //Act
            ResultHandler<Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>>> result = service.GetOrderBy<EventModel>(orderByStatement);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }
        [Test] public void GetOrderBy_WhenOrderByStatementNotValid_ShouldReturnFail()
        {
            //Arrange
            OrderByStatement<EventModel> orderByStatement = TestsFacade.OrderBytatementFacade.BuildOrderByStatement<EventModel>();
            orderByStatement.PropertyName = Guid.NewGuid().ToString();            

            OrderByService service = new OrderByService();

            //Act
            ResultHandler<Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>>> result = service.GetOrderBy<EventModel>(orderByStatement);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }

        [TearDown] public void CleanUp()
        {
        }
    }

}