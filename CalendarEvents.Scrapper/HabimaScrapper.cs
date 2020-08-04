
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CalendarEvents.Models;


using HtmlAgilityPack;

namespace CalendarEvents.Scrapper
{
    public class HabimaScrapper : Scrapper
    {
        private static string URL = "https://www.habima.co.il/%d7%9c%d7%95%d7%97-%d7%94%d7%a6%d7%92%d7%95%d7%aa/";
        private string proxy;
        public HabimaScrapper(string proxy = null)
        {
            this.proxy = proxy;
        }


        public override List<EventModel> Scrape()
        {
            var pageSource = string.Empty;
            var request = PrepareRequest(URL, proxy);
            var source = GetPageSource(request);
            return CollectEventsFromPageSource(source);

        }

        private List<EventModel> CollectEventsFromPageSource(string source)
        {
            SHA256 mySHA256 = SHA256.Create();
            
            var results = new List<EventModel>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);
            var eventsTable = doc.DocumentNode.SelectSingleNode("//table");
            var tableRows = eventsTable.SelectNodes("//tr");

            for (int i = 1; i < tableRows.Count; i++)
            {
                var rowCells = tableRows[i].SelectNodes("td");
                EventModel e = new EventModel();
                e.Title = rowCells[0].InnerText;
                e.Start = GetDateFromString(rowCells[2].InnerText, rowCells[3].InnerText);
                e.URL = rowCells[5].FirstChild.Attributes["href"].Value;
                byte[] shaBytes = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(e.URL + e.Title + e.Start.ToString())); // Create base64 
                e.Base64Id = Convert.ToBase64String(shaBytes);
                e.Id = Guid.NewGuid();
                results.Add(e);
            }

            return results;
        }

        private DateTime GetDateFromString(string stringDate, string stringTime)
        {
            try
            {
                string[] dateParts = stringDate.Split('-');
                // Habima dates are days/months/years
                int day = Convert.ToInt32(dateParts[0]);
                int month = Convert.ToInt32(dateParts[1]);
                int year = Convert.ToInt32(dateParts[2]);

                string[] timeParts = stringTime.Split(':');
                int hour = Convert.ToInt32(timeParts[0]);
                int minutes = Convert.ToInt32(timeParts[1]);
                


                return new DateTime(year, month, day, hour, minutes, 0, DateTimeKind.Local);
            }

            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
}
