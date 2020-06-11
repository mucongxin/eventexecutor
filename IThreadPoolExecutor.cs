using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventExecutor
{
    /// <summary>
    /// 
    /// </summary>
    public interface IThreadPoolExecutor : IScheduledExecutorService
    {
        /// <summary>
        /// Returns list of owned event executors.
        /// </summary>
        IEnumerable<IEventExecutor> Items { get; }

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

        /// <summary>
        /// Returns <see cref="IEventExecutor"/>.
        /// </summary>
        IEventExecutor GetNext();
    }
}
