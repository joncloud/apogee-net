namespace Apogee
{
    interface IBatchQueue<T>
    {
        int Count { get; }
        void Enqueue(T item);
        bool TryDequeue(out Batch<T> batch);
    }
}
