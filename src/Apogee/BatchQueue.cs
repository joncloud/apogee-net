using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Apogee
{
    class BatchQueue<T> : IBatchQueue<T>
    {
        readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        public int Count => _items.Count;

        public void Enqueue(T item) =>
            _items.Enqueue(item);

        public bool TryDequeue(out Batch<T> batch)
        {
            batch = default(Batch<T>);

            var items = new List<T>();
            while (_items.TryDequeue(out var item))
            {
                items.Add(item);
            }

            bool any = items.Any();
            if (any) batch = new Batch<T>(Guid.NewGuid(), items.AsReadOnly());
            return any;
        }
    }
}
