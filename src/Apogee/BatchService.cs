namespace Apogee
{
    class BatchService<T> : IBatchService<T>
    {
        readonly IBatchQueue<T> _queue;
        readonly IBatchNotifier<T> _batchNotifier;
        public BatchService(IApogeeFlusher flusher, IBatchQueue<T> queue, IBatchNotifier<T> batchNotifier)
        {
            flusher.Register(Flush);
            _queue = queue;
            _batchNotifier = batchNotifier;
        }

        public void Add(T item)
        {
            _queue.Enqueue(item);
            _batchNotifier.Notify();
        }

        public void Flush() =>
            _batchNotifier.Flush();
    }
}
