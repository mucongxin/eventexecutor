using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace EventExecutor
{
    public class CompatibleConcurrentQueue<T> : ConcurrentQueue<T>, IQueue<T>
    {
        public bool TryEnqueue(T element)
        {
            this.Enqueue(element);
            return true;
        }

        void IQueue<T>.Clear()
        {
            T item;
            while (this.TryDequeue(out item))
            {
            }
        }
    }
}
