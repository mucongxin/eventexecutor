
using System;

namespace EventExecutor
{
    sealed class StateActionScheduledTask : ScheduledTask
    {
        readonly Action<object> action;

        public StateActionScheduledTask(AbstractScheduledEventExecutor executor, Action<object> action, object state, PreciseTimeSpan deadline)
            : base(executor, deadline, new TaskCompletionSource(state))
        {
            this.action = action;
        }

        protected override void Execute() => this.action(this.Completion.AsyncState);
    }
}