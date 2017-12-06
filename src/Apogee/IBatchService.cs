namespace Apogee
{
    public interface IBatchService<T>
    {
        void Add(T item);
        void Flush();
    }
}
