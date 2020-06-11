using System;

namespace EventExecutor
{
    public class RejectedExecutionException : Exception
    {
        public RejectedExecutionException()
        {
        }

        public RejectedExecutionException(string message)
            : base(message)
        {
        }
    }
}