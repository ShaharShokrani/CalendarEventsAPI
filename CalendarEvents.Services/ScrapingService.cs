using System;
using System.Linq;
using System.Threading;
using CalendarEvents.Models;
using CalendarEvents.Scrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CalendarEvents.Services
{
    public interface IScrapingService
    {
        void Start();
        void Stop();
    }
    public class ScrapingService : IScrapingService // , IHostedService
    {
        private readonly ILogger<ScrapingService> log;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public event EventHandler EventsUpdated;
        private HabimaScrapper habimaScrapper;
        private Thread _thread;
        CancellationTokenSource _cancellationToken;

        //TODO: get interval from configuration
        private int _miliSecondsBetweenIterations = 60 * 60 * 1000; 

        public ScrapingService(IServiceScopeFactory serviceScopeFactory, ILogger<ScrapingService> log)
        {
            // _eventsService = eventsService;
            this.serviceScopeFactory = serviceScopeFactory;
            habimaScrapper = new HabimaScrapper();
            _cancellationToken = new CancellationTokenSource();
            _thread = new Thread(MainLoop);
            _thread.Name = "ScrapingService";
            this.log = log;
        }


        private void MainLoop()
        {
            log.LogInformation("MainLoop Started");
            
            // TODO: Make last scraping time persistent
            // Scraping logic not so good, scraping duplicate or not updating frequently

            DateTime lastScraped = DateTime.MinValue; 
            do
            {
                try
                {
                    using (var scope = serviceScopeFactory.CreateScope()) // Used becuse DI disposing scoped service on background worker
                    {
                        var _eventsService = scope.ServiceProvider.GetService<IGenericService<EventModel>>();
                        var scrapedEvents = habimaScrapper.ScrapeFromDate(lastScraped);
                        if (!scrapedEvents.Any())
                            continue;

                        foreach (var e in scrapedEvents)
                            _eventsService.Insert(e); // We need to deny insert of duplicates on base64Id

                        EventsUpdated?.Invoke(this, null);

                        lastScraped = scrapedEvents.OrderByDescending(e => e.Start).First().Start.ToUniversalTime() ;
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Failed to add events from scrapper");
                }
            }
            while (!_cancellationToken.Token.WaitHandle.WaitOne());

            log.LogInformation("MainLoop Ended");
        }


        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _cancellationToken.Cancel();
            _thread.Join();
        }

        
    }
}