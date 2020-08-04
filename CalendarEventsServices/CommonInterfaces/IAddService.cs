namespace CalendarEvents.Services
{
    public interface IInsertService<T>
    {
        ResultService Insert(T obj);
    }
}
