using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalendarEvents.Scrapper
{
    public interface IScrapper<T>
    {
        List<T> Scrape();
    }
}
