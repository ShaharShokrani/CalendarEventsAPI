using CalendarEvents.Controllers;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Mvc;
using Autofac.Extras.Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using CalendarEvents.DataAccess;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace CalendarEvents.Tests
{
    public class EventsControllerIntegrationTests
    {        
        private AutoMock _mock = null;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;

        public EventsControllerIntegrationTests()
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .UseEnvironment(Consts.TestingEnvironment)
                .UseStartup<Startup>();

            var server = new TestServer(builder);
            _context = server.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            _client = server.CreateClient();
        }

        [SetUp] public void Setup()
        {
            _mock = AutoMock.GetLoose();
        }

        #region Get
        [Test] public async void Get_WhenCalled_ShouldReturnOk()
        {
            // Arrange
            EventModel expectedItem = TestsFacade.EventsFacade.BuildEventModels().First();            
            _context.Events.Add(expectedItem);
            _context.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/api/events/{expectedItem.Id}");

            // Assert
            Assert.Equals(HttpStatusCode.OK, response.StatusCode);
            string jsonResult = await response.Content.ReadAsStringAsync();
            EventModelDTO resultItem = JsonConvert.DeserializeObject<EventModelDTO>(jsonResult);
            Assert.Equals(expectedItem.Id, resultItem.Id);
        }
        #endregion

        [TearDown] public void CleanUp()
        {
            if (_mock != null)
                _mock.Dispose();
        }
    }
}