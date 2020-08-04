//using CalendarEvents.Controllers;
//using CalendarEvents.Models;
//using CalendarEvents.Services;
//using Microsoft.AspNetCore.Mvc;
//using Autofac.Extras.Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Routing;
//using CalendarEvents.Tests;
//using CalendarEvents.DataAccess;
//using System.IO;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
//using Moq;
//using System.Linq;
//using System.Linq.Expressions;

//namespace CalendarEvents.Repository.Tests
//{
//    public class EventsServiceTests
//    {
//        private AutoMock _mock = null;

//        [SetUp]
//        public void Setup()
//        {
//            _mock = AutoMock.GetLoose();
//        }

//        #region Insert
//        [Test] public void Insert_WhenCalled_ShouldCallDbSetContextAdd()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);
//            context
//                .Setup(c => c.SaveChanges());

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Insert(expectedItem);

//            //Assert
//            dbSetMock.Verify(x => x.Add(It.Is<EventModel>(y => y == expectedItem)), Times.Once, "Add failed");
//            context.Verify(x => x.SaveChanges(), Times.Once, "Save failed");
//        }       
//        #endregion  

//        #region Get
//        [Test] public void Get_WhenCalled_ShouldReturnList()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList();

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Provider).Returns(expectedList.AsQueryable().Provider);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Expression).Returns(expectedList.AsQueryable().Expression);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.ElementType).Returns(expectedList.AsQueryable().ElementType);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.GetEnumerator()).Returns(expectedList.AsQueryable().GetEnumerator());

//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            IEnumerable<EventModel> resultList = repository.Get(null, null, "");

//            //Assert
//            Assert.AreEqual(expectedList, resultList);
//        }
//        [Test] public void Get_WhenCalledWithFilter_ShouldReturnList()
//        {
//            //Arrange
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList(20);
//            EventModel filteredItem = expectedList.ToList()[0];

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Provider).Returns(expectedList.AsQueryable().Provider);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Expression).Returns(expectedList.AsQueryable().Expression);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.ElementType).Returns(expectedList.AsQueryable().ElementType);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.GetEnumerator()).Returns(expectedList.AsQueryable().GetEnumerator());

//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            IEnumerable<EventModel> resultList = repository.Get(x => x.Id == filteredItem.Id, null, "");

//            //Assert
//            Assert.IsTrue(resultList.Count() == 1);
//            Assert.AreEqual(expectedList.Where(x => x.Id == filteredItem.Id), resultList);
//        }
//        [Test] public void Get_WhenCalledWithOrder_ShouldReturnList()
//        {
//            //Arrange
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList(2);

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Provider).Returns(expectedList.AsQueryable().Provider);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Expression).Returns(expectedList.AsQueryable().Expression);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.ElementType).Returns(expectedList.AsQueryable().ElementType);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.GetEnumerator()).Returns(expectedList.AsQueryable().GetEnumerator());

//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            IEnumerable<EventModel> resultList = repository.Get(null, query => query.OrderBy(e => e.Id), "");

//            //Assert
//            Assert.AreEqual(expectedList.OrderBy(e=>e.Id), resultList);
//        }
//        [Test] public void Get_WhenCalledWithInclude_ShouldReturnList()
//        {
//            //Arrange
//            IEnumerable<EventModel> expectedList = TestsFacade.EventsFacade.BuildEventModelList(2);

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Provider).Returns(expectedList.AsQueryable().Provider);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.Expression).Returns(expectedList.AsQueryable().Expression);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.ElementType).Returns(expectedList.AsQueryable().ElementType);
//            dbSetMock.As<IQueryable<EventModel>>().Setup(x => x.GetEnumerator()).Returns(expectedList.AsQueryable().GetEnumerator());

//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            IEnumerable<EventModel> resultList = repository.Get(null, null, "Id");

//            //Assert
//            Assert.AreEqual(expectedList.AsQueryable().Include(e => e.Id), resultList);
//        }
//        #endregion

//        #region GetById
//        [Test] public void GetById_WhenCalled_ShouldReturnItem()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            var dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock
//                .Setup(db => db.Find(expectedItem.Id))
//                .Returns(expectedItem);

//            var context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            EventModel resultItem = repository.GetById(expectedItem.Id);

//            //Assert
//            Assert.AreEqual(expectedItem, resultItem);
//        }
//        #endregion

//        #region Remove
//        [Test] public void Remove_WhenCalled_ShouldCallDbSetContextRemove()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            Mock<DbSet<EventModel>> dbSetMock = _mock.Mock<DbSet<EventModel>>();

//            dbSetMock
//                .Setup(db => db.Find(expectedItem.Id))
//                .Returns(expectedItem);

//            Mock<ICalendarDbContext> context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);
//            context
//                .Setup(c => c.GetEntityState<EventModel>(expectedItem))
//                .Returns(EntityState.Added);
//            context
//                .Setup(c => c.SaveChanges());

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Remove(expectedItem.Id);

//            //Assert
//            context.Verify(x => x.GetEntityState<EventModel>(expectedItem), Times.Once, "GetEntityState Failed");
//            context.Verify(x => x.SaveChanges(), Times.Once, "SaveChanges failed");

//            dbSetMock.Verify(x => x.Remove(It.Is<EventModel>(y => y == expectedItem)), Times.Once, "Remove Failed");
//            dbSetMock.Verify(x => x.Find(It.Is<Guid>(y => y == expectedItem.Id)), Times.Once, "Find Failed");
//        }
//        [Test] public void Remove_NotFind_ShouldDoNothing()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            Mock<DbSet<EventModel>> dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            Mock<ICalendarDbContext> context = _mock.Mock<ICalendarDbContext>();

//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Remove(expectedItem.Id);

//            //Assert
//            context.Verify(x => x.GetEntityState<EventModel>(expectedItem), Times.Never, "GetEntityState Failed");

//            dbSetMock.Verify(x => x.Find(It.Is<Guid>(y => y == expectedItem.Id)), Times.Once, "Find Failed");
//            dbSetMock.Verify(x => x.Remove(It.Is<EventModel>(y => y == expectedItem)), Times.Never, "Remove Failed");
//        }
//        [Test] public void Remove_Detached_ShouldCallAttachAndRemove()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            Mock<DbSet<EventModel>> dbSetMock = _mock.Mock<DbSet<EventModel>>();
//            dbSetMock
//                .Setup(db => db.Find(expectedItem.Id))
//                .Returns(expectedItem);

//            Mock<ICalendarDbContext> context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);
//            context
//                .Setup(c => c.GetEntityState<EventModel>(expectedItem))
//                .Returns(EntityState.Detached);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Remove(expectedItem.Id);

//            //Assert
//            context.Verify(x => x.GetEntityState<EventModel>(expectedItem), Times.Once, "GetEntityState Failed");

//            dbSetMock.Verify(x => x.Remove(It.Is<EventModel>(y => y == expectedItem)), Times.Once, "Remove Failed");
//            dbSetMock.Verify(x => x.Attach(It.Is<EventModel>(y => y == expectedItem)), Times.Once, "Attach Failed");
//            dbSetMock.Verify(x => x.Find(It.Is<Guid>(y => y == expectedItem.Id)), Times.Once, "Find Failed");
//        }
//        #endregion

//        #region Update
//        [Test] public void Update_WhenCalled_ShouldCallDbSetContextUpdate()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            Mock<DbSet<EventModel>> dbSetMock = _mock.Mock<DbSet<EventModel>>();

//            Mock<ICalendarDbContext> context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.SetValues<EventModel>(expectedItem, It.IsAny<EventModel>()));
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);
//            context
//                .Setup(c => c.SaveChanges());

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Update(expectedItem, new EventModel());

//            //Assert
//            context.Verify(x => x.SetValues<EventModel>(expectedItem, It.IsAny<EventModel>()), Times.Once, "SetValues Failed");
//            context.Verify(x => x.SaveChanges(), Times.Once, "SaveChanges failed");            
//        }
//        [Test] public void Update_NotFind_ShouldDoNothing()
//        {
//            //Arrange
//            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModelItem();

//            Mock<DbSet<EventModel>> dbSetMock = _mock.Mock<DbSet<EventModel>>();

//            Mock<ICalendarDbContext> context = _mock.Mock<ICalendarDbContext>();
//            context
//                .Setup(c => c.Set<EventModel>())
//                .Returns(dbSetMock.Object);

//            GenericRepository<EventModel> repository = _mock.Create<GenericRepository<EventModel>>();

//            //Act
//            repository.Update(null, null);

//            //Assert
//            context.Verify(x => x.SetValues<EventModel>(expectedItem, It.IsAny<EventModel>()), Times.Never, "SetValues Failed");
//        }
//        #endregion

//        [TearDown]
//        public void CleanUp()
//        {
//            if (_mock != null)
//                _mock.Dispose();
//        }
//    }
//}