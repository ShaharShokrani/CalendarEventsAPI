using Autofac.Extras.Moq;
using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using CalendarEvents.Tests;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarEvents.Services.Tests
{
    public class GenericServiceTests
    {
        private AutoMock _mock = null;
        private IEnumerable<EventModel> _eventModels;
        private IAsyncEnumerable<EventModel> _asyncEnumerable;
        private EventModel _eventModel;
        private GenericService<EventModel> _genericService;
        private Exception _exception;
        private Mock<IGenericRepository<EventModel>> _repositoryMock;
        private Mock<ILogger<GenericService<EventModel>>> _loggerMock;

        //private async Task Main()
        //{
        //    await foreach (var dataPoint in FetchIOTData())
        //    {                
        //    }            
        //}

        [SetUp]
        public void Setup()
        {
            this._mock = AutoMock.GetLoose();
            this._eventModels = TestsFacade.EventsFacade.BuildEventModels();
            this._asyncEnumerable = TestsFacade.EventsFacade.BuildIAsyncEnumerable(this._eventModels);
            this._eventModel = this._eventModels.First();
            this._exception = new Exception(Guid.NewGuid().ToString());

            this._repositoryMock = this._mock.Mock<IGenericRepository<EventModel>>();
            this._loggerMock = this._mock.Mock<ILogger<GenericService<EventModel>>>();
            this._loggerMock.MockLog(LogLevel.Error);
            this._loggerMock.MockLog(LogLevel.Information);
            this._loggerMock.MockLog(LogLevel.Warning);
            this._genericService = new GenericService<EventModel>(this._repositoryMock.Object, this._loggerMock.Object);
            this._genericService = _mock.Create<GenericService<EventModel>>();
        }

        #region Insert
        [Test] public async Task Insert_WhenCalled_ShouldNewItem()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.InsertRange(this._eventModels))
                .Returns(Task.CompletedTask);

            //Act
            ResultHandler result = await this._genericService.InsertRange(this._eventModels);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResultHandler>(result);
            Assert.IsTrue(result.Success);
        }
        [Test] public async Task Add_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.InsertRange(this._eventModels))
                .Throws(this._exception);

            //Act
            var actual = await this._genericService.InsertRange(this._eventModels);

            //Assert  
            AssertException(actual);
        }
        #endregion

        #region Get
        //[Test]
        //public async Task Get_WhenCalled_ShouldReturnList()
        //{
        //    //Arrange
        //    this._repositoryMock
        //        .Setup(items => items.Get(null, null, "", null, null))
        //        .Returns(() => this._asyncEnumerable);

        //    //Act
        //    var actual = this._genericService.Get(null, null, "");

        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<ResultHandler<IEnumerable<EventModel>>>(result);
        //    Assert.IsTrue(result.Success);
        //    Assert.IsInstanceOf<IEnumerable<EventModel>>(result.Value);

        //    IEnumerable<EventModel> resultList = result.Value;

        //    Assert.IsNotNull(resultList);
        //    Assert.AreEqual(this._eventModels.Count(), resultList.Count());
        //    CollectionAssert.AreEqual(this._eventModels, resultList);
        //}
        //[Test]
        //public async Task Get_WhenCalledWithFilter_ShouldReturnList()
        //{
        //    //Arrange
        //    EventModel filteredItem = expectedList.ToList()[0];

        //    this._repositoryMock
        //        .Setup(items => items.Get(It.IsAny<Expression<Func<EventModel, bool>>>(), null, "", null, null))
        //        .Returns(() => Task.FromResult(this._eventModels));                

        //    //Act
        //    var actual = await this._genericService.Get(null, null, "");

        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<ResultHandler<IEnumerable<EventModel>>>(result);
        //    Assert.IsTrue(result.Success);
        //    Assert.IsInstanceOf<IEnumerable<EventModel>>(result.Value);

        //    IEnumerable<EventModel> resultList = result.Value;

        //    Assert.IsNotNull(resultList);
        //    Assert.AreEqual(resultList.Count(), this._eventModels.Count());
        //    Assert.AreEqual(resultList.First().GetHashCode(), this._eventModels.First().GetHashCode());
        //}
        [Test] public async Task Get_WhenCalledWithOrder_ShouldReturnList()
        {
            //Arrange
            OrderByStatement<EventModel> orderByStatement = TestsFacade.OrderBytatementFacade.BuildOrderByStatement<EventModel>();

            this._repositoryMock
                .Setup(items => items.Get(null, It.IsAny<Func<IQueryable<EventModel>, IOrderedQueryable<EventModel>>>(), "", null, null))
                .Returns(() => this._asyncEnumerable);

            //Act
            var actual = this._genericService.Get(null, orderByStatement, "");

            //Assert
            await AssertSuccessGet(actual);
        }        
        [Test] public async Task Get_WhenCalledWithInclude_ShouldReturnList()
        {
            //Arrange
            string include = "Id";

            this._repositoryMock
                .Setup(items => items.Get(null, null, include, null, null))
                .Returns(() => this._asyncEnumerable);

            //Act
            var actual = this._genericService.Get(null, null, include);

            //Assert
            await AssertSuccessGet(actual);
        }
        [Test] public void Get_WhenRepositoryReturnsNull_ShouldReturnNotFound()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.Get(null, null, "", null, null))
                .Returns(() => null);

            //Act
            var actual = this._genericService.Get(null, null, "");

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsNull(actual.Value);
            Assert.IsTrue(actual.ErrorCode == ErrorCode.NotFound);
        }
        [Test] public void Get_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.Get(null, null, "", null, null))
                .Throws(this._exception);

            //Act
            var actual = this._genericService.Get(null, null, "");

            //Assert
            AssertResultHandlerException(actual);
        }
        private async Task AssertSuccessGet(ResultHandler<IAsyncEnumerable<EventModel>> actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ResultHandler<IAsyncEnumerable<EventModel>>>(actual);
            Assert.IsTrue(actual.Success);
            Assert.IsInstanceOf<IAsyncEnumerable<EventModel>>(actual.Value);

            IAsyncEnumerable<EventModel> actualEventModels = actual.Value;

            Assert.IsNotNull(actualEventModels);
            await foreach (var actualEventModel in actualEventModels)
            {
                Assert.IsTrue(this._eventModels.Any(eventModel => eventModel.Id == actualEventModel.Id));
            }
        }
        #endregion

        #region GetById
        [Test] public async Task GetById_WhenCalled_ShouldReturnItem()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.GetById(this._eventModel.Id))
                .Returns(() => Task.FromResult(this._eventModel));

            //Act
            var actual = await this._genericService.GetById(this._eventModel.Id);

            //Assert
            Assert.IsNotNull(actual );
            Assert.IsInstanceOf<ResultHandler<EventModel>>(actual);
            Assert.IsTrue(actual.Success);
            Assert.IsInstanceOf<EventModel>(actual.Value);

            EventModel eventModel = actual.Value as EventModel;

            Assert.IsNotNull(eventModel);
            Assert.AreEqual(this._eventModel.GetHashCode(), eventModel.GetHashCode());
        }
        [Test] public async Task GetById_WhenRepositoryReturnsNull_ShouldReturnNotFound()
        {
            //Arrange
            Guid id = new Guid();

            this._repositoryMock
                .Setup(items => items.GetById(id))
                .Returns(() => Task.FromResult<EventModel>(null));

            //Act
            var actual = await this._genericService.GetById(id);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsNull(actual.Value);
            Assert.IsTrue(actual.ErrorCode == ErrorCode.NotFound);
        }
        [Test] public async Task GetById_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            Guid id = new Guid();
            this._repositoryMock
                .Setup(items => items.GetById(id))
                .Throws(this._exception);

            //Act
            var actual = await this._genericService.GetById(id);

            //Assert
            AssertResultHandlerException<EventModel>(actual);
        }
        #endregion

        #region Delete
        [Test] public async Task Delete_WhenCalled_ShouldReturnOk()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.GetById(this._eventModel.Id))
                .Returns(() => Task.FromResult(this._eventModel));

            this._repositoryMock
                .Setup(items => items.Remove(this._eventModel))
                .Returns(Task.CompletedTask);

            //Act
            var actual = await this._genericService.Delete(this._eventModel.Id);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ResultHandler>(actual);
            Assert.IsTrue(actual.Success);
        }
        [Test] public async Task Delete_WhenRepositoryReturnsNull_ShouldReturnNotFound()
        {
            //Arrange
            Guid id = new Guid();

            this._repositoryMock
                .Setup(items => items.GetById(id))
                .Returns(() => Task.FromResult<EventModel>(null));

            //Act
            var actual = await this._genericService.Delete(id);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsTrue(actual.ErrorCode == ErrorCode.NotFound);
        }
        [Test] public async Task Delete_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            Guid id = new Guid();

            this._repositoryMock
                .Setup(items => items.GetById(id))
                .Throws(this._exception);

            //Act
            var actual = await this._genericService.Delete(id);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsTrue(actual.Exception == this._exception);
        }
        #endregion

        #region Update
        [Test] public async Task Update_WhenCalled_ShouldReturnItem()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.GetById(this._eventModel.Id))
                .Returns(Task.FromResult(this._eventModel));

            //Act
            var actual = await this._genericService.Update(this._eventModel);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ResultHandler>(actual);
            Assert.IsTrue(actual.Success);
        }
        [Test] public async Task Update_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.GetById(this._eventModel.Id))
                .Returns(Task.FromResult(this._eventModel));
            this._repositoryMock
                .Setup(items => items.Update(It.IsAny<EventModel>()))
                .Throws(this._exception);

            //Act
            var actual = await this._genericService.Update(this._eventModel);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsTrue(actual.Exception == this._exception, "Not the same Expection");
        }
        #endregion

        #region Update
        [Test] public async Task IsOwner_WhenCalled_ShouldReturnItem()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.IsOwner(this._eventModel.Id, this._eventModel.OwnerId))
                .Returns(Task.FromResult(true));

            //Act
            var actual = await this._genericService.IsOwner(this._eventModel.Id, this._eventModel.OwnerId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ResultHandler>(actual);
            Assert.IsTrue(actual.Success);
        }
        [Test] public async Task IsOwner_WhenFalse_ShouldFail()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.IsOwner(this._eventModel.Id, this._eventModel.OwnerId))
                .Returns(Task.FromResult(false));

            //Act
            var actual = await this._genericService.IsOwner(this._eventModel.Id, this._eventModel.OwnerId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ResultHandler>(actual);
            Assert.IsFalse(actual.Success);
        }
        [Test] public async Task IsOwner_WhenRepositoryThrowException_ShouldReturnError()
        {
            //Arrange
            this._repositoryMock
                .Setup(items => items.IsOwner(It.IsAny<Guid>(), It.IsAny<string>()))
                .Throws(this._exception);

            //Act
            var actual = await this._genericService.IsOwner(new Guid(), null);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.IsTrue(actual.Exception == this._exception, "Not the same Expection");
        }
        #endregion
        [TearDown]
        public void CleanUp()
        {
            if (_mock != null)
                _mock.Dispose();
        }

        private void AssertResultHandlerException<T>(ResultHandler<T> resultService)
        {
            Assert.IsNotNull(resultService);
            Assert.IsFalse(resultService.Success);
            Assert.IsNull(resultService.Value);
            Assert.IsTrue(resultService.Exception == this._exception);
        }
        private void AssertException(ResultHandler actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Success);
            Assert.AreEqual(this._exception, actual.Exception);
        }
    }
}