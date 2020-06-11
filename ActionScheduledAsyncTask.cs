
using System;
using System.Threading;

namespace EventExecutor
{
    sealed class ActionScheduledAsyncTask : ScheduledAsyncTask
    {
        readonly Action action;

        public ActionScheduledAsyncTask(AbstractScheduledEventExecutor executor, Action action, PreciseTimeSpan deadline, CancellationToken cancellationToken)
            : base(executor, deadline, new TaskCompletionSource(), cancellationToken)
        {
            this.action = action;
        }

        protected override void Execute() => this.action();
    }
}