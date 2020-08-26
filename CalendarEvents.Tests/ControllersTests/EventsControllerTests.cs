using CalendarEvents.Controllers;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Mvc;
using Autofac.Extras.Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CalendarEvents.Tests
{
    public class EventsControllerTests
    {
        private AutoMock _mock = null;
        private EventsController _controller;
        
        private Mock<ILogger<EventsController>> _loggerMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IGenericService<EventModel>> _genericServiceMock;        
        
        private IEnumerable<EventModel> _eventModels;        
        private IEnumerable<EventModelDTO> _eventModelsDTOs;
        private List<EventModelPostDTO> _eventModelPostsDTOs;

        [SetUp]
        public void Setup()
        {
            _mock = AutoMock.GetLoose();
            this._controller = _mock.Create<EventsController>();
            this._eventModels = TestsFacade.EventsFacade.BuildEventModels();            
            this._eventModelsDTOs = TestsFacade.EventsFacade.BuildEventModelDTOList(this._eventModels);
            this._eventModelPostsDTOs = TestsFacade.EventsFacade.BuildEventModelPostsDTOs(this._eventModels);

            this._loggerMock = this._mock.Mock<ILogger<EventsController>>();
            this._loggerMock.MockLog(LogLevel.Error);
            this._loggerMock.MockLog(LogLevel.Information);
            this._loggerMock.MockLog(LogLevel.Warning);

            this._userServiceMock = this._mock.Mock<IUserService>();
            this._genericServiceMock = this._mock.Mock<IGenericService<EventModel>>();

            this._mock.Mock<IMapper>()
                .Setup(map => map.Map<GetRequest<EventModel>>(It.IsAny<GetRequest<EventModelDTO>>()))
                .Returns(() => new GetRequest<EventModel>());
            this._mock.Mock<IMapper>()
                .Setup(map => map.Map<IEnumerable<EventModel>>(It.IsAny<IEnumerable<EventModelPostDTO>>()))
                .Returns(() => this._eventModels);
            this._mock.Mock<IMapper>()
                .Setup(map => map.Map<IEnumerable<EventModelDTO>>(It.IsAny<IEnumerable<EventModel>>()))
                .Returns(() => this._eventModelsDTOs);
            this._mock.Mock<IMapper>()
                .Setup(map => map.Map<IEnumerable<EventModelPostDTO>>(It.IsAny<IEnumerable<EventModel>>()))
                .Returns(() => this._eventModelPostsDTOs);
            this._mock.Mock<IMapper>()
                .Setup(map => map.Map<EventModelDTO>(It.IsAny<EventModel>()))
                .Returns(() => this._eventModelsDTOs.First());
        }

        #region Get
        [Test]
        public async Task Get_WhenCalled_ShouldReturnOk()
        {
            //Arrange                       
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Ok(this._eventModels);

            this._genericServiceMock
                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
                                        It.IsAny<OrderByStatement<EventModel>>(),
                                        It.IsAny<string>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Get(new GetRequest<EventModelDTO>());

            //Assert
            AssertHttpCode<OkObjectResult>(actual, this._eventModelsDTOs.GetHashCode());            
        }
        [Test]
        public async Task Get_WhenCalledWithNull_ShouldReturnOk()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Ok(this._eventModels);

            this._genericServiceMock
                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
                                        It.IsAny<OrderByStatement<EventModel>>(),
                                        It.IsAny<string>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Get(null);

            //Assert
            AssertHttpCode<OkObjectResult>(actual, this._eventModelsDTOs.GetHashCode());
        }
        [Test]
        public async Task Get_WhenServiceHasError_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
                                        It.IsAny<OrderByStatement<EventModel>>(),
                                        It.IsAny<string>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Get(new GetRequest<EventModelDTO>());

            //Assert                       
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());            
        }
        [Test]
        public async Task Get_WhenServiceThrowException_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
                                        It.IsAny<OrderByStatement<EventModel>>(),
                                        It.IsAny<string>()))
                .Throws(new Exception());                

            //Act
            IActionResult actual = await this._controller.Get(new GetRequest<EventModelDTO>());

            //Assert       
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        #endregion

        #region GetById
        [Test]
        public async Task GetById_RequestNotValid_ShouldReturnBadRequest()
        {
            //Arrange
            //Act
            IActionResult actual = await this._controller.Get(new Guid());

            //Assert
            AssertHttpCode<BadRequestObjectResult>(actual, 0);
        }
        [Test]
        public async Task GetById_WhenCalled_ShouldReturnOk()
        {
            //Arrange
            ResultHandler<EventModel> expected = ResultHandler.Ok(this._eventModels.First());
            this._genericServiceMock
                .Setup(items => items.GetById(expected.Value.Id))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Get(expected.Value.Id);

            //Assert
            AssertHttpCode<OkObjectResult>(actual, this._eventModelsDTOs.First().GetHashCode());
        }
        [Test]
        public async Task GetById_WhenServiceHasError_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<EventModel> expected = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.GetById(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(expected));
           
            //Act
            IActionResult actual = await this._controller.Get(Guid.NewGuid());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        [Test]
        public async Task GetById_WhenServiceThrowException_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.GetById(It.IsAny<Guid>()))
                .Throws(new Exception());            

            //Act
            IActionResult actual = await this._controller.Get(new GetRequest<EventModelDTO>());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());            
        }
        #endregion

        #region Post
        [Test]
        public async Task Post_RequestNotValid_ShouldReturnBadRequest()
        {
            //Arrange
            this._controller.ModelState.AddModelError(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            
            //Act
            IActionResult actual = await this._controller.Post(null);

            //Assert
            AssertHttpCode<BadRequestObjectResult>(actual, 0);
        }
        [Test]
        public async Task Post_WhenCalled_ShouldReturnPost()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Ok();

            this._genericServiceMock
                .Setup(items => items.InsertRange(It.IsAny<IEnumerable<EventModel>>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Post(this._eventModelPostsDTOs);

            //Assert            
            AssertHttpCode<CreatedAtActionResult>(actual, this._eventModelPostsDTOs.GetHashCode());
        }
        [Test]
        public async Task Post_WhenServiceHasError_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Fail(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.InsertRange(this._eventModels))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Post(this._eventModelPostsDTOs);

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        [Test]
        public async Task Post_WhenServiceThrowException_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Fail(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.InsertRange(It.IsAny<IEnumerable<EventModel>>()))
                .Throws(new Exception());

            //Act
            IActionResult actual = await this._controller.Post(null);

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        #endregion

        #region Put
        [Test]
        public async Task Put_RequestNotValid_ShouldReturnBadRequest()
        {
            //Arrange
            this._controller.ModelState.AddModelError(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            IActionResult actual = await this._controller.Put(null);

            //Assert
            AssertHttpCode<BadRequestObjectResult>(actual, 0);
        }
        [Test]
        public async Task Put_WhenCalled_ShouldReturnPut()
        {
            //Arrange
            ResultHandler<EventModel> expected = ResultHandler.Ok(this._eventModels.First());            
            this._genericServiceMock
                .Setup(x => x.GetById(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(expected));

            this._genericServiceMock
                .Setup(x => x.Update(It.IsAny<EventModel>()))
                .Returns(() => Task.FromResult(ResultHandler.Ok()));

            this._userServiceMock
                .Setup(x => x.OwnerId)
                .Returns(Guid.Parse(this._eventModels.First().OwnerId));

            //Act
            IActionResult actual = await this._controller.Put(new EventPutRequest());

            //Assert
            AssertHttpCode<OkObjectResult>(actual, this._eventModelsDTOs.First().GetHashCode());            
        }
        [Test]
        public async Task Put_WhenDifferentOwnerId_ShouldReturnUnauthorized()
        {
            //Arrange
            ResultHandler<EventModel> expected = ResultHandler.Ok(this._eventModels.First());
            this._genericServiceMock
                .Setup(x => x.GetById(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(expected));

            this._genericServiceMock
                .Setup(x => x.Update(It.IsAny<EventModel>()))
                .Returns(() => Task.FromResult(ResultHandler.Ok()));

            this._userServiceMock
                .Setup(x => x.OwnerId)
                .Returns(Guid.NewGuid());

            //Act
            EventPutRequest eventPutRequest = new EventPutRequest();
            IActionResult actual = await this._controller.Put(eventPutRequest);

            //Assert
            AssertHttpCode<UnauthorizedObjectResult>(actual, eventPutRequest.GetHashCode());
        }
        [Test]
        public async Task Put_WhenServiceHasError_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Fail(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Update(It.IsAny<EventModel>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Put(new EventPutRequest());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        [Test]
        public async Task Put_WhenServiceThrowException_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Update(It.IsAny<EventModel>()))
                .Throws(new Exception());

            //Act
            IActionResult actual = await this._controller.Put(new EventPutRequest());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        #endregion

        #region Delete
        [Test]
        public async Task Delete_RequestNotValid_ShouldReturnBadRequest()
        {
            //Arrange
            this._controller.ModelState.AddModelError(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            IActionResult actual = await this._controller.Delete(new Guid());

            //Assert
            AssertHttpCode<BadRequestObjectResult>(actual, 0);
        }
        [Test]
        public async Task Delete_WhenCalled_ShouldReturnOk()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Ok();

            this._genericServiceMock
                .Setup(items => items.Delete(this._eventModels.First().Id))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Delete(this._eventModels.First().Id);

            //Assert            
            AssertHttpCode<OkObjectResult>(actual, this._eventModels.First().Id.GetHashCode());
        }
        [Test]
        public async Task Delete_WhenServiceHasError_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler expected = ResultHandler.Fail(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Delete(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(expected));

            //Act
            IActionResult actual = await this._controller.Delete(Guid.NewGuid());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        [Test]
        public async Task Delete_WhenServiceThrowException_ShouldReturnStatusCode500()
        {
            //Arrange
            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

            this._genericServiceMock
                .Setup(items => items.Delete(It.IsAny<Guid>()))
                .Throws(new Exception());

            //Act
            IActionResult actual = await this._controller.Delete(Guid.NewGuid());

            //Assert
            AssertHttpCode<ObjectResult>(actual, expected.ErrorCode.GetHashCode());
        }
        #endregion

        private void AssertHttpCode<T>(T expected, IActionResult actual, int hashCode) where T : ObjectResult
        {
            Assert.IsNotNull(actual);
            T actualObjectResult = actual as T;
            switch (actualObjectResult)
            {
                case BadRequestObjectResult x:
                    Assert.AreEqual(400, x.StatusCode);
                    break;
                case OkObjectResult x:
                    Assert.AreEqual(200, x.StatusCode);
                    break;
                case UnauthorizedObjectResult x:
                    Assert.AreEqual(401, x.StatusCode);
                    break;
                case CreatedAtActionResult x:
                    Assert.AreEqual(201, x.StatusCode);
                    break;                    
                case ObjectResult x:
                    Assert.AreEqual(500, x.StatusCode);
                    break;
                default:
                    throw new NotImplementedException();
            }            
            if (hashCode != 0)
                Assert.AreEqual(actualObjectResult.Value.GetHashCode(), hashCode);
        }                   
    }
}