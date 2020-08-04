using CalendarEvents.Models;
using Autofac.Extras.Moq;
using NUnit.Framework;
using System.Collections.Generic;
using CalendarEvents.Tests;
using System.Linq.Expressions;
using System;

namespace CalendarEvents.Services.Tests
{
    public class FiltersServiceTests
    {
        private AutoMock _mock = null;

        [SetUp]
        public void Setup()
        {
            _mock = AutoMock.GetLoose();
        }

        //1. Is Ok
        //2. throws exception
        //3. propertyName is not valid
        //4. Value is not convertable to propertyName
        //5. Operation is not valid.

        [Test] public void BuildExpression_WhenCalled_ShouldReturnOk()
        {
            //Arrange
            IEnumerable<FilterStatement<EventModel>> filterStatements = TestsFacade.FilterStatementFacade.BuildFilterStatementList<EventModel>();
            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResultHandler<Expression<Func<EventModel, bool>>>>(result);
            Assert.IsTrue(result.Success);
        }

        [Test] public void BuildExpression_WhenPropertyNotValid_ShouldReturnFail()
        {
            //Arrange
            FilterStatement<EventModel> filterStatement = TestsFacade.FilterStatementFacade.BuildFilterStatement<EventModel>();
            filterStatement.PropertyName = Guid.NewGuid().ToString();
            IEnumerable<FilterStatement<EventModel>> filterStatements = new List<FilterStatement<EventModel>>()
            {
                filterStatement
            };

            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }

        [Test] public void BuildExpression_WhenPropertyNull_ShouldReturnFail()
        {
            //Arrange
            FilterStatement<EventModel> filterStatement = TestsFacade.FilterStatementFacade.BuildFilterStatement<EventModel>();
            filterStatement.PropertyName = null;
            IEnumerable<FilterStatement<EventModel>> filterStatements = new List<FilterStatement<EventModel>>()
            {
                filterStatement
            };

            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }

        [Test] public void BuildExpression_WhenValueNotValid_ShouldReturnFail()
        {
            //Arrange
            FilterStatement<EventModel> filterStatement = TestsFacade.FilterStatementFacade.BuildFilterStatement<EventModel>();
            filterStatement.Value = 1;
            filterStatement.PropertyName = "Id";
            IEnumerable<FilterStatement<EventModel>> filterStatements = new List<FilterStatement<EventModel>>()
            {
                filterStatement
            };

            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }

        [Test] public void BuildExpression_WhenValueNull_ShouldReturnFail()
        {
            //Arrange
            FilterStatement<EventModel> filterStatement = TestsFacade.FilterStatementFacade.BuildFilterStatement<EventModel>();
            filterStatement.Value = null;
            filterStatement.PropertyName = "Id";
            IEnumerable<FilterStatement<EventModel>> filterStatements = new List<FilterStatement<EventModel>>()
            {
                filterStatement
            };

            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }


        [Test] public void BuildExpression_WhenOperationNotValid_ShouldReturnFail()
        {
            //Arrange
            FilterStatement<EventModel> filterStatement = TestsFacade.FilterStatementFacade.BuildFilterStatement<EventModel>();
            filterStatement.Operation = FilterOperation.Undefined;
            IEnumerable<FilterStatement<EventModel>> filterStatements = new List<FilterStatement<EventModel>>()
            {
                filterStatement
            };

            FiltersService<EventModel> service = new FiltersService<EventModel>(filterStatements);

            //Act
            ResultHandler<Expression<Func<EventModel, bool>>> result = service.BuildExpression();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Value);
            Assert.IsTrue(result.ErrorCode == ErrorCode.EntityNotValid);
        }

        [TearDown] public void CleanUp()
        {
            if (_mock != null)
                _mock.Dispose();
        }
    }

}