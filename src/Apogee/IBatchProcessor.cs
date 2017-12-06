namespace Apogee
{
    public interface IBatchProcessor<T>
    {
        void Process(Batch<T> batch);
    }
}
