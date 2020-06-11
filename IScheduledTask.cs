using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventExecutor
{
    public interface IScheduledTask
    {
        bool Cancel();

        PreciseTimeSpan Deadline { get; }

        Task Completion { get; }

        TaskAwaiter GetAwaiter();
    }
}
