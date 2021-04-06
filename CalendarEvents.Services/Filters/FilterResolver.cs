using CalendarEvents.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarEvents.Services
{
    public delegate IFilter<EventModel> EventFilterResolver(FilterType type);
}
