//using CalendarEvents.Controllers;
//using CalendarEvents.Models;
//using CalendarEvents.Services;
//using Microsoft.AspNetCore.Mvc;
//using Autofac.Extras.Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Routing;
//using System.Linq;
//using Moq;
//using AutoMapper;

//namespace CalendarEvents.Tests
//{
//    public class EventsControllerTests
//    {
//        private AutoMock _mock = null;

//        [SetUp] public void Setup()
//        {
//            _mock = AutoMock.GetLoose();
//        }

//        #region Get
//        [Test] public void Get_WhenCalled_ShouldReturnOk()
//        {
//            //Arrange
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList();
//            IEnumerable<EventModelDTO> expectedListDTO = TestsFacade.EventsFacade.BuildEventModelDTOList();
//            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Ok(expectedList);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
//                                        It.IsAny<OrderByStatement<EventModel>>(),
//                                        It.IsAny<string>()))
//                .Returns(() => expected);
//            _mock.Mock<IMapper>()
//                .Setup(map => map.Map<GetRequest<EventModel>>(It.IsAny<GetRequest<EventModelDTO>>()))
//                .Returns(() => new GetRequest<EventModel>());

//            _mock.Mock<IMapper>()
//                .Setup(map => map.Map<IEnumerable<EventModelDTO>>(It.IsAny<IEnumerable<EventModel>>()))
//                .Returns(() => expectedListDTO);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModelDTO>> result = await controller.Get(new GetRequest<EventModelDTO>());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<OkObjectResult>(result.Result);

//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult.Value);

//            IEnumerable<EventModelDTO> resultListDTO = okResult.Value as IEnumerable<EventModelDTO>;

//            Assert.IsNotNull(resultListDTO);
//            Assert.AreEqual(resultListDTO.Count(), expectedListDTO.Count());
//            Assert.AreEqual(resultListDTO.GetHashCode(), expectedListDTO.GetHashCode());
//        }
//        [Test] public void Get_WhenCalledWithNull_ShouldReturnOk()
//        {
//            //Arrange
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList();
//            IEnumerable<EventModelDTO> expectedListDTO = TestsFacade.EventsFacade.BuildEventModelDTOList();
//            ResultHandler<IEnumerable<EventModel>> expected = ResultHandler.Ok(expectedList);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
//                                        It.IsAny<OrderByStatement<EventModel>>(),
//                                        It.IsAny<string>()))
//                .Returns(() => expected);
//            _mock.Mock<IMapper>()
//                .Setup(map => map.Map<GetRequest<EventModel>>(It.IsAny<GetRequest<EventModelDTO>>()))
//                .Returns(() => new GetRequest<EventModel>());

//            _mock.Mock<IMapper>()
//                .Setup(map => map.Map<IEnumerable<EventModelDTO>>(It.IsAny<IEnumerable<EventModel>>()))
//                .Returns(() => expectedListDTO);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModelDTO>> result = controller.Get(null);

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<OkObjectResult>(result.Result);

//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult.Value);

//            IEnumerable<EventModelDTO> resultListDTO = okResult.Value as IEnumerable<EventModelDTO>;

//            Assert.IsNotNull(resultListDTO);
//            Assert.AreEqual(resultListDTO.Count(), expectedListDTO.Count());
//            Assert.AreEqual(resultListDTO, expectedListDTO);
//        }
//        [Test] public void Get_WhenServiceHasError_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<IEnumerable<EventModel>> expectedResultService = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
//                                        It.IsAny<OrderByStatement<EventModel>>(),
//                                        It.IsAny<string>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModelDTO>> result = controller.Get(new GetRequest<EventModelDTO>());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        [Test] public void Get_WhenServiceThrowException_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<IEnumerable<EventModel>> expectedResultService = ResultHandler.Fail<IEnumerable<EventModel>>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Get(It.IsAny<IEnumerable<FilterStatement<EventModel>>>(),
//                                        It.IsAny<OrderByStatement<EventModel>>(),
//                                        It.IsAny<string>()))
//                .Throws(new Exception());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModelDTO>> result = controller.Get(new GetRequest<EventModelDTO>());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;            
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        #endregion

//        #region GetById
//        [Test] public void GetById_RequestNotValid_ShouldReturnBadRequest()
//        {
//            //Arrange
//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModelDTO> actionResult = controller.Get(new Guid());

//            //Assert
//            AssertBadRequestResult(actionResult);
//        }
//        [Test] public void GetById_WhenCalled_ShouldReturnOk()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Ok(expectedItem);
//            EventModelDTO expectedItemDTO = TestsFacade.EventsFacade.BuildEventModelDTOItem();

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.GetById(It.IsAny<Guid>()))
//                .Returns(() => expectedResultService);

//            _mock.Mock<IMapper>()
//                .Setup(items => items.Map<EventModelDTO>(It.IsAny<EventModel>()))
//                .Returns(() => expectedItemDTO);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModelDTO> actionResult = controller.Get(Guid.NewGuid());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<OkObjectResult>(actionResult.Result);

//            var okResult = actionResult.Result as OkObjectResult;
//            Assert.IsNotNull(okResult.Value);

//            EventModelDTO okResultItem = okResult.Value as EventModelDTO;

//            Assert.IsNotNull(okResultItem);
//            Assert.AreEqual(okResultItem.GetHashCode(), expectedItemDTO.GetHashCode());
//        }
//        [Test] public void GetById_WhenServiceHasError_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.GetById(It.IsAny<Guid>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModelDTO> actionResult = controller.Get(Guid.NewGuid());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<ObjectResult>(actionResult.Result);

//            var objectResult = actionResult.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        [Test] public void GetById_WhenServiceThrowException_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.GetById(It.IsAny<Guid>()))
//                .Throws(new Exception());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModelDTO>> result = controller.Get(new GetRequest<EventModelDTO>());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        #endregion

//        #region Post
//        [Test] public void Post_RequestNotValid_ShouldReturnBadRequest()
//        {
//            //Arrange
//            var controller = _mock.Create<EventsController>();

//            //Act
//            controller.ModelState.AddModelError("Title", "Required");
//            ActionResult actionResult = controller.Post(new EventPostRequest());

//            //Assert
//            AssertBadRequestResult(actionResult);
//        }
//        [Test] public void Post_WhenCalled_ShouldReturnPost()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Ok(expectedItem);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Insert(It.IsAny<EventModel>()))
//                .Returns(() => expectedResultService);
//            _mock.Mock<IMapper>()
//                .Setup(map => map.Map<EventModel>(It.IsAny<EventPostRequest>()))
//                .Returns(() => new EventModel());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModel> actionResult = controller.Post(new EventPostRequest());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult.Result);

//            var createdAtActionResult = actionResult.Result as CreatedAtActionResult;
//            Assert.IsNotNull(createdAtActionResult.Value);

//            Guid createdAtActionResultItem = (Guid)createdAtActionResult.Value;
//            RouteValueDictionary createdAtActionResultRouteValues = createdAtActionResult.RouteValues;

//            Assert.IsNotNull(createdAtActionResultItem);
//            Assert.AreEqual(createdAtActionResult.ActionName, "Post");
//            Assert.AreEqual(createdAtActionResultRouteValues["Id"], createdAtActionResultItem);
//        }
//        [Test] public void Post_WhenServiceHasError_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Insert(It.IsAny<EventModel>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModel> actionResult = controller.Post(new EventPostRequest());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<ObjectResult>(actionResult.Result);

//            var objectResult = actionResult.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        [Test] public void Post_WhenServiceThrowException_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Insert(It.IsAny<EventModel>()))
//                .Throws(new Exception());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModel>> result = controller.Post(new EventPostRequest());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        #endregion

//        #region Put
//        [Test] public void Put_RequestStateNotValid_ShouldReturnBadRequest()
//        {
//            //Arrange
//            var controller = _mock.Create<EventsController>();

//            //Act
//            controller.ModelState.AddModelError("Title", "Required");
//            ActionResult actionResult = controller.Put(new EventPutRequest());

//            //Assert
//            AssertBadRequestResult(actionResult);
//        }
//        [Test] public void Put_RequestIdNotValid_ShouldReturnBadRequest()
//        {
//            //Arrange
//            var controller = _mock.Create<EventsController>();

//            //Act
//            controller.ModelState.AddModelError("Id", "Required");
//            ActionResult<EventModel> actionResult = controller.Put(new EventPutRequest());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);
//        }
//        [Test] public void Put_WhenCalled_ShouldReturnPut()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Ok(new EventModel());

//            Mock<IGenericService<EventModel>> service = _mock.Mock<IGenericService<EventModel>>();
//            service
//                .Setup(items => items.GetById(It.IsAny<Guid>()))
//                .Returns(() => expectedResultService);
//            service
//                .Setup(items => items.Update(It.IsAny<EventModel>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult actionResult = controller.Put(new EventPutRequest());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<OkResult>(actionResult);
//        }
//        [Test] public void Put_WhenServiceHasError_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);            

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Update(It.IsAny<EventModel>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<EventModel> actionResult = controller.Put(new EventPutRequest());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<ObjectResult>(actionResult.Result);

//            var objectResult = actionResult.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        [Test] public void Put_WhenServiceThrowException_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Update(It.IsAny<EventModel>()))
//                .Throws(new Exception());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModel>> result = controller.Put(new EventPutRequest());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        #endregion

//        #region Delete
//        [Test] public void Delete_RequestNotValid_ShouldReturnBadRequest()
//        {
//            //Arrange
//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult actionResult = controller.Delete(new Guid());

//            //Assert
//            AssertBadRequestResult(actionResult);
//        }
//        [Test] public void Delete_WhenCalled_ShouldReturnOk()
//        {
//            //Arrange
//            ResultHandler expectedResultService = ResultHandler.Ok();

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Delete(It.IsAny<Guid>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult actionResult = controller.Delete(Guid.NewGuid());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<OkResult>(actionResult);
//        }
//        [Test] public void Delete_WhenServiceHasError_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler expectedResultService = ResultHandler.Fail(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Delete(It.IsAny<Guid>()))
//                .Returns(() => expectedResultService);

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult actionResult = controller.Delete(Guid.NewGuid());

//            //Assert
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<ObjectResult>(actionResult);

//            var objectResult = actionResult as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        [Test] public void Delete_WhenServiceThrowException_ShouldReturnStatusCode500()
//        {
//            //Arrange
//            ResultHandler<EventModel> expectedResultService = ResultHandler.Fail<EventModel>(ErrorCode.Unknown);

//            _mock.Mock<IGenericService<EventModel>>()
//                .Setup(items => items.Delete(It.IsAny<Guid>()))
//                .Throws(new Exception());

//            var controller = _mock.Create<EventsController>();

//            //Act
//            ActionResult<IEnumerable<EventModel>> result = controller.Delete(Guid.NewGuid());

//            //Assert
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);

//            ObjectResult objectResult = result.Result as ObjectResult;
//            AssertStatusCode500(objectResult, expectedResultService.ErrorCode);
//        }
//        #endregion

//        [TearDown] public void CleanUp()
//        {
//            if (_mock != null)
//                _mock.Dispose();
//        }

//        private void AssertBadRequestResult(ActionResult<EventModelDTO> actionResult)
//        {
//            Assert.IsNotNull(actionResult);
//            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);
//        }
//        private void AssertStatusCode500(ObjectResult objectResult, ErrorCode errorCode)
//        {
//            Assert.IsNotNull(objectResult.Value);
//            Assert.IsNotNull(objectResult.StatusCode);
//            Assert.AreEqual(objectResult.StatusCode, 500);
//            Assert.AreEqual((ErrorCode)objectResult.Value, errorCode);
//        }
//    }
//}