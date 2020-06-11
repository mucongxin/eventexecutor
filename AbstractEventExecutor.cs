using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thread = EventExecutor.XThread;


namespace EventExecutor
{
    /// <summary>
    ///     Abstract base class for <see cref="IEventExecutor" /> implementations
    /// </summary>
    public abstract class AbstractEventExecutor : AbstractExecutorService, IEventExecutor
    {
        static readonly TimeSpan DefaultShutdownQuietPeriod = TimeSpan.FromSeconds(2);
        static readonly TimeSpan DefaultShutdownTimeout = TimeSpan.FromSeconds(15);

        /// <summary>Creates an instance of <see cref="AbstractEventExecutor"/>.</summary>
        protected AbstractEventExecutor()
            : this(null)
        {
        }

        /// <summary>Creates an instance of <see cref="AbstractEventExecutor"/>.</summary>
        protected AbstractEventExecutor(IThreadPoolExecutor pool)
        {
            this.Pool = pool;
        }

        /// <inheritdoc cref="IEventExecutorGroup"/>
        public abstract bool IsShuttingDown { get; }

        /// <inheritdoc cref="IEventExecutorGroup"/>
        public abstract Task TerminationCompletion { get; }

        /// <inheritdoc cref="IEventExecutorGroup"/>
        public IEventExecutor GetNext() => this;

        /// <inheritdoc cref="IEventExecutor"/>
        public IThreadPoolExecutor Pool { get; }

        /// <inheritdoc cref="IEventExecutor"/>
        public bool InEventLoop => this.IsInEventLoop(Thread.CurrentThread);

        /// <inheritdoc cref="IEventExecutor" />
        public IEnumerable<IEventExecutor> Items => this.GetItems();

        protected abstract IEnumerable<IEventExecutor> GetItems();

        /// <inheritdoc cref="IEventExecutor"/>
        public abstract bool IsInEventLoop(Thread thread);

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual IScheduledTask Schedule(IRunnable action, TimeSpan delay)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual IScheduledTask Schedule(Action action, TimeSpan delay)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual IScheduledTask Schedule(Action<object> action, object state, TimeSpan delay)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual IScheduledTask Schedule(Action<object, object> action, object context, object state, TimeSpan delay)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(Action action, TimeSpan delay) =>
            this.ScheduleAsync(action, delay, CancellationToken.None);

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(Action<object> action, object state, TimeSpan delay, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(Action<object> action, object state, TimeSpan delay) =>
            this.ScheduleAsync(action, state, delay, CancellationToken.None);

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(Action action, TimeSpan delay, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(Action<object, object> action, object context, object state, TimeSpan delay) =>
            this.ScheduleAsync(action, context, state, delay, CancellationToken.None);

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public virtual Task ScheduleAsync(
            Action<object, object> action,
            object context,
            object state,
            TimeSpan delay,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public Task ShutdownGracefullyAsync() => this.ShutdownGracefullyAsync(DefaultShutdownQuietPeriod, DefaultShutdownTimeout);

        /// <inheritdoc cref="IScheduledExecutorService"/>
        public abstract Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan timeout);
        
        protected static void SafeExecute(IRunnable task)
        {
            try
            {
                task.Run();
            }
            catch (Exception ex)
            {
                //Logger.Warn("A task raised an exception. Task: {}", task, ex);
            }
        }
    }
}
