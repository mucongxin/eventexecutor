using System;
using System.Collections.Generic;
using System.Text;

namespace EventExecutor
{
    public interface IScheduledRunnable : IRunnable, IScheduledTask, IComparable<IScheduledRunnable>
    {
    }
}
