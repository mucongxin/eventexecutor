using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventExecutor
{
    public sealed class ThreadPoolExecutor : AbstractThreadPoolExecutor, IThreadPoolExecutor
    {
        static readonly int DefaultEventLoopThreadCount = Environment.ProcessorCount * 2;
        static readonly Func<IThreadPoolExecutor, IEventExecutor> DefaultEventLoopFactory = pool => new SingleThreadEventExecutor(pool);

        public static IThreadPoolExecutor Default = new ThreadPoolExecutor();

        readonly IEventExecutor[] eventExecutors;
        int requestId;

        public override bool IsShutdown => eventExecutors.All(eventLoop => eventLoop.IsShutdown);

        public override bool IsTerminated => eventExecutors.All(eventLoop => eventLoop.IsTerminated);

        public override bool IsShuttingDown => eventExecutors.All(eventLoop => eventLoop.IsShuttingDown);

        /// <inheritdoc />
        public override Task TerminationCompletion { get; }

        /// <inheritdoc />
        protected override IEnumerable<IEventExecutor> GetItems() => this.eventExecutors;

        /// <inheritdoc />
        public new IEnumerable<IEventExecutor> Items => this.eventExecutors;

        /// <summary>Creates a new instance of <see cref="ThreadPoolExecutor"/>.</summary>
        public ThreadPoolExecutor()
            : this(DefaultEventLoopFactory, DefaultEventLoopThreadCount)
        {
        }

        /// <summary>Creates a new instance of <see cref="ThreadPoolExecutor"/>.</summary>
        public ThreadPoolExecutor(int eventLoopCount)
            : this(DefaultEventLoopFactory, eventLoopCount)
        {
        }

        /// <summary>Creates a new instance of <see cref="ThreadPoolExecutor"/>.</summary>
        public ThreadPoolExecutor(Func<IThreadPoolExecutor, IEventExecutor> eventLoopFactory)
            : this(eventLoopFactory, DefaultEventLoopThreadCount)
        {
        }

        /// <summary>Creates a new instance of <see cref="ThreadPoolExecutor"/>.</summary>
        public ThreadPoolExecutor(Func<IThreadPoolExecutor, IEventExecutor> eventLoopFactory, int eventLoopCount)
        {
            this.eventExecutors = new IEventExecutor[eventLoopCount];
            var terminationTasks = new Task[eventLoopCount];
            for (int i = 0; i < eventLoopCount; i++)
            {
                IEventExecutor eventLoop;
                bool success = false;
                try
                {
                    eventLoop = eventLoopFactory(this);
                    success = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("failed to create a child event loop.", ex);
                }
                finally
                {
                    if (!success)
                    {
                        Task.WhenAll(
                                this.eventExecutors
                                    .Take(i)
                                    .Select(loop => loop.ShutdownGracefullyAsync()))
                            .Wait();
                    }
                }

                this.eventExecutors[i] = eventLoop;
                terminationTasks[i] = eventLoop.TerminationCompletion;
            }
            this.TerminationCompletion = Task.WhenAll(terminationTasks);
        }

        /// <inheritdoc />
        IEventExecutor IThreadPoolExecutor.GetNext() => this.GetNext();

        /// <inheritdoc />
        public override IEventExecutor GetNext()
        {
            int id = Interlocked.Increment(ref this.requestId);
            return this.eventExecutors[Math.Abs(id % this.eventExecutors.Length)];
        }

        /// <inheritdoc cref="IEventExecutorGroup.ShutdownGracefullyAsync()" />
        public override Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan timeout)
        {
            foreach (IEventExecutor eventLoop in this.eventExecutors)
            {
                eventLoop.ShutdownGracefullyAsync(quietPeriod, timeout);
            }
            return this.TerminationCompletion;
        }
    }
}
