using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventExecutor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public delegate void XParameterizedThreadStart(object obj);
    /// <summary>
    /// 
    /// </summary>
    public delegate void XThreadStart();
    /// <summary>
    /// 
    /// </summary>
    public sealed class XThread : CriticalFinalizerObject
    {
        [ThreadStatic]
        private static XThread _currentThread;
        static int maxThreadId;
        static int GetNewThreadId() => Interlocked.Increment(ref maxThreadId);

        readonly int threadId;
        Task task;
        readonly EventWaitHandle completed = new EventWaitHandle(false, EventResetMode.AutoReset);
        readonly EventWaitHandle readyToStart = new EventWaitHandle(false, EventResetMode.AutoReset);
        object startupParameter;

        public static XThread CurrentThread => _currentThread ?? (_currentThread = new XThread());
        /// <summary>
        /// 
        /// </summary>
        public int ManagedThreadId => this.threadId;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAlive { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        XThread()
        {
            this.threadId = GetNewThreadId();
            this.IsAlive = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public XThread(Action action)
            : this()
        {
            this.CreateLongRunningTask(x => action());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public XThread(XParameterizedThreadStart start)
            : this()
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }
            this.CreateLongRunningTask(start);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this.readyToStart.Set();
            this.IsAlive = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadStartFunc"></param>
        void CreateLongRunningTask(XParameterizedThreadStart threadStartFunc)
        {
            this.task = Task.Factory.StartNew(
                () =>
                {
                    // We start the task running, then unleash it by signaling the readyToStart event.
                    // This is needed to avoid thread reuse for tasks (see below)
                    this.readyToStart.WaitOne();
                    // This is the first time we're using this thread, therefore the TLS slot must be empty
                    if (_currentThread != null)
                    {
                        Debug.WriteLine("warning: currentThread already created; OS thread reused");
                        Debug.Assert(false);
                    }
                    _currentThread = this;
                    threadStartFunc(this.startupParameter);
                    this.completed.Set();
                },
                CancellationToken.None,
                // .NET always creates a brand new thread for LongRunning tasks
                // This is not documented but unlikely to ever change:
                // https://github.com/dotnet/corefx/issues/2576#issuecomment-126693306
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Start(object parameter)
        {
            this.startupParameter = parameter;
            this.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Join(TimeSpan timeout) => this.completed.WaitOne(timeout);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Join(int millisecondsTimeout) => this.completed.WaitOne(millisecondsTimeout);

        public static void Sleep(int millisecondsTimeout) => Task.Delay(millisecondsTimeout).Wait();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is XThread thread)
            {
                return thread.ManagedThreadId == this.ManagedThreadId;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return threadId;
        }
    }
}
