using System;
using System.Collections.Generic;
using System.Text;

namespace EventExecutor
{
    public interface IQueue<T>
    {
        bool TryEnqueue(T item);

        bool TryDequeue(out T item);

        bool TryPeek(out T item);

        int Count { get; }

        bool IsEmpty { get; }

        void Clear();
    }
}
