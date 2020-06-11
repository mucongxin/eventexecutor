
using System;
using System.Threading;

namespace EventExecutor
{
    sealed class StateActionWithContextScheduledAsyncTask : ScheduledAsyncTask
    {
        readonly Action<object, object> action;
        readonly object context;

        public StateActionWithContextScheduledAsyncTask(AbstractScheduledEventExecutor executor, Action<object, object> action, object context, object state,
            PreciseTimeSpan deadline, CancellationToken cancellationToken)
            : base(executor, deadline, new TaskCompletionSource(state), cancellationToken)
        {
            this.action = action;
            this.context = context;
        }

        protected override void Execute() => this.action(this.context, this.Completion.AsyncState);
    }
}