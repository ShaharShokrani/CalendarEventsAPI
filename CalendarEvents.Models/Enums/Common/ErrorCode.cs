namespace CalendarEvents.Models
{
    public enum ErrorCode
    {
        Undefined = 0,
        Exception = 1,        
        EntityNotFound = 3,
        Unauthorized = 4,
        ValidationError = 5
    }
}