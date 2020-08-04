namespace CalendarEvents.Models
{
    public enum FilterOperation
    {
        Undefined = 0,
        Equal = 1,
        Contains = 2,
        StartsWith = 3,
        EndsWith = 4,
        NotEqual = 5,
        GreaterThan = 6,
        GreaterThanOrEqual = 7,
        LessThan = 8,
        LessThanOrEqual = 9
    }
}