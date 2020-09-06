using CalendarEvents.Models;
using Autofac.Extras.Moq;
using NUnit.Framework;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using CalendarEvents.DataAccess;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using IdentityModel.Client;
using System.Text;

namespace CalendarEvents.Tests
{
    public class EventsControllerIntegrationTests
    {        
        private AutoMock _mock = null;
        private EventModel _expectedItem;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _client;        

        public EventsControllerIntegrationTests()
        {
            try
            {
                IWebHostBuilder builder = new WebHostBuilder()
                    .UseEnvironment(Consts.TestingEnvironment)
                    .UseStartup<Startup>();

                var server = new TestServer(builder);
                _context = server.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
                _client = server.CreateClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [SetUp] public void Setup()
        {
            _mock = AutoMock.GetLoose();
        }

        #region Get
        [Test]        
        public async Task Get_WhenCalled_ShouldReturnOk()
        {
            try
            {
                // Arrange
                this._expectedItem = TestsFacade.EventsFacade.BuildEventModels().First();
                _context.Events.Add(this._expectedItem);
                await _context.SaveChangesAsync();

                HttpMethod httpMethod = HttpMethod.Post;
                Uri uri = new Uri("http://localhost/api/events/get/");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, uri);                
                httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(new { }), Encoding.UTF8, "application/json");

                HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
                CancellationToken cancellationToken = new CancellationToken();

                // Act                
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await _client.SendAsync(httpRequestMessage, httpCompletionOption, cancellationToken);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonResult = await response.Content.ReadAsStringAsync();
                EventModelDTO[] resultItems = JsonConvert.DeserializeObject<EventModelDTO[]>(jsonResult);

                Assert.IsTrue(resultItems.Any(r => r.Id == this._expectedItem.Id));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        [TestCase("eyJhbGciOiJSUzI1NiIsImtpZCI6IlpMOHUzV09XVlI4WkdRTlZueGMySFEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE1OTkwNzU1MDYsImV4cCI6MTU5OTA3NjEwNiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAyIiwiYXVkIjoiY2FsZW5kYXJldmVudHNhcGkiLCJjbGllbnRfaWQiOiJjYWxlbmRhcmV2ZW50c3VpIiwic3ViIjoiYjMzZjY4MWUtZGY3OS00NTZjLWIwNDgtYzlmYzNlOTQxNmU5IiwiYXV0aF90aW1lIjoxNTk3OTM0OTAyLCJpZHAiOiJsb2NhbCIsInNjb3BlIjpbIm9wZW5pZCIsImVtYWlsIiwicHJvZmlsZSIsImNhbGVuZGFyZXZlbnRzYXBpIl0sImFtciI6WyJwd2QiXX0.Kl2x6chGHa3ieu3dwnPMRCy77XwU1xV8oF6I3pd_8Ny70XJggohKfA6Mr58E2oB7sT2yTwRL0MpCjGuO5eJSiyQP2NLm7V2CCTBke-zJcKQQ2RLreJVxGUl_ytp_D8aGWhVRYAxc0RLZ60YvXGqyJaiMf2El9UWjgjq7LbjfTuzjM2mi5vdueTh8xzbvtMU2lVMnTeXM6q0W1fdmF72JTl6qjxHJ45Vw-gnVugej8gv8bRPSOrhK01Zji7fFfrroi19YVAT1LK5bccgrM3GoGq6boH2_quNLoQj8IQSPXY2iZBJ94ZL4saulnqzmlBJ2lqrRrAciGv_CEwI6Hp3trA")]
        public async Task GetById_WhenCalled_ShouldReturnOk(string access_token)
        {
            try
            {
                // Arrange
                this._expectedItem = TestsFacade.EventsFacade.BuildEventModels().First();
                _context.Events.Add(this._expectedItem);
                await _context.SaveChangesAsync();

                HttpMethod httpMethod = HttpMethod.Get;
                Uri uri = new Uri($"http://localhost/api/events/getbyid?id={_expectedItem.Id}");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, uri);
                httpRequestMessage.SetBearerToken(access_token);

                HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead;
                CancellationToken cancellationToken = new CancellationToken();

                // Act                
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await _client.SendAsync(httpRequestMessage, httpCompletionOption, cancellationToken);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonResult = await response.Content.ReadAsStringAsync();
                EventModelDTO resultItem = JsonConvert.DeserializeObject<EventModelDTO>(jsonResult);

                Assert.IsTrue(resultItem.Id == this._expectedItem.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        [TearDown] public async Task CleanUp()
        {
            _context.Events.Remove(this._expectedItem);
            await _context.SaveChangesAsync();
        }
    }
}