using System;
using System.Collections.Generic;

namespace Apogee
{
    public struct Batch<T>
    {
        public Guid Id { get; }
        public IReadOnlyList<T> Items { get; }

        public Batch(Guid id, IReadOnlyList<T> items)
        {
            Id = id;
            Items = items;
        }
    }
}
