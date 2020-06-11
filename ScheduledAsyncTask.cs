using System.Threading;

namespace EventExecutor
{
    abstract class ScheduledAsyncTask : ScheduledTask
    {
        readonly CancellationToken cancellationToken;
        CancellationTokenRegistration cancellationTokenRegistration;

        protected ScheduledAsyncTask(AbstractScheduledEventExecutor executor, PreciseTimeSpan deadline, TaskCompletionSource promise, CancellationToken cancellationToken)
            : base(executor, deadline, promise)
        {
            this.cancellationToken = cancellationToken;
            this.cancellationTokenRegistration = cancellationToken.Register(s => ((ScheduledAsyncTask)s).Cancel(), this);
        }

        public override void Run()
        {
            this.cancellationTokenRegistration.Dispose();
            if (this.cancellationToken.IsCancellationRequested)
            {
                this.Promise.TrySetCanceled();
            }
            else
            {
                base.Run();
            }
        }
    }
}