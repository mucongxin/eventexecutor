using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Thread = EventExecutor.XThread;

namespace EventExecutor
{
    public interface IEventExecutor : IScheduledExecutorService
    {
        /// <summary>
        /// 
        /// </summary>
        IThreadPoolExecutor Pool { get; }

        /// <summary>
        ///     Returns <c>true</c> if the current <see cref="Thread" /> belongs to this event loop,
        ///     <c>false</c> otherwise.
        /// </summary>
        /// <remarks>
        ///     It is a convenient way to determine whether code can be executed directly or if it
        ///     should be posted for execution to this executor instance explicitly to ensure execution in the loop.
        /// </remarks>
        bool InEventLoop { get; }

        /// <summary>
        ///     Returns <c>true</c> if the given <see cref="Thread" /> belongs to this event loop,
        ///     <c>false></c> otherwise.
        /// </summary>
        bool IsInEventLoop(Thread thread);

        /// <summary>
        ///     Returns <c>true</c> if and only if this executor is being shut down via <see cref="ShutdownGracefullyAsync()" />.
        /// </summary>
        bool IsShuttingDown { get; }
        
        /// <summary>
        /// Terminates this <see cref="IEventExecutorGroup"/> and all its <see cref="IEventExecutor"/>s.
        /// </summary>
        /// <returns><see cref="Task"/> for completion of termination.</returns>
        Task ShutdownGracefullyAsync();

        /// <summary>
        /// Terminates this <see cref="IEventExecutorGroup"/> and all its <see cref="IEventExecutor"/>s.
        /// </summary>
        /// <returns><see cref="Task"/> for completion of termination.</returns>
        Task ShutdownGracefullyAsync(TimeSpan quietPeriod, TimeSpan timeout);

        /// <summary>
        /// A <see cref="Task"/> for completion of termination. <see cref="ShutdownGracefullyAsync()"/>.
        /// </summary>
        Task TerminationCompletion { get; }

    }
}
