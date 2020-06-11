using System;
using System.Threading;

namespace EventExecutor
{
    sealed class StateActionScheduledAsyncTask : ScheduledAsyncTask
    {
        readonly Action<object> action;

        public StateActionScheduledAsyncTask(AbstractScheduledEventExecutor executor, Action<object> action, object state, PreciseTimeSpan deadline,
            CancellationToken cancellationToken)
            : base(executor, deadline, new TaskCompletionSource(state), cancellationToken)
        {
            this.action = action;
        }

        protected override void Execute() => this.action(this.Completion.AsyncState);
    }
}