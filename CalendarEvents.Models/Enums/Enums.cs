namespace CalendarEvents.Models
{
    public enum FilterType
    {
        Undefined = 0,
        MultiCheckbox = 1
    }
    public enum AudienceType
    {
        Undefined = 0,
        Adult = 1,
        Senior = 2,
        Youth = 3,
        Children = 4,
        Infants = 5
    }
    /// <summary>
    /// ISO 4217
    /// </summary>
    public enum CurrencyISO
    {
        Undefined = 0,
        ILS = 376,
        USD = 840,
        EUR = 978,
        GBP = 826
    }
}
