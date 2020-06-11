
using System;

namespace EventExecutor
{
    sealed class ActionScheduledTask : ScheduledTask
    {
        readonly Action action;

        public ActionScheduledTask(AbstractScheduledEventExecutor executor, Action action, PreciseTimeSpan deadline)
            : base(executor, deadline, new TaskCompletionSource())
        {
            this.action = action;
        }

        protected override void Execute() => this.action();
    }
}